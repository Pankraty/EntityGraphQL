using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityGraphQL.Compiler;

namespace EntityGraphQL.AspNet.WebSockets
{
    /// <summary>
    /// Ties the GraphQL subscription to the WebSocket connection.
    /// 
    /// As GraphQL says nothing about the protocol or the delivery of the stream.
    /// </summary>
    /// <typeparam name="TEventType"></typeparam>
    internal class WebSocketSubscription<TEventType> : IWebSocketSubscription, IObserver<TEventType>
    {
        private readonly Guid id;
        private readonly IGraphQLWebSocketServer server;
        private readonly GraphQLSubscriptionStatement subscriptionStatement;
        private readonly GraphQLSubscriptionField subscriptionNode;

        private readonly IObservable<TEventType>? observable;
        private readonly IDisposable? subscription;
        private readonly WebSocketSubscriptionMode mode = WebSocketSubscriptionMode.None;
        private readonly object? subscriptionTask;

        public WebSocketSubscription(Guid id, object subscriptionResult, IGraphQLWebSocketServer server, GraphQLSubscriptionStatement subscriptionStatement, GraphQLSubscriptionField node)
        {
            this.id = id;
            this.server = server;
            this.subscriptionStatement = subscriptionStatement;
            this.subscriptionNode = node;
            if (subscriptionResult.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IObservable<>)))
            {
                mode = WebSocketSubscriptionMode.Observable;
                this.observable = (IObservable<TEventType>)subscriptionResult;
                this.subscription = this.observable.Subscribe(this);
            }
            else
            {
                mode = WebSocketSubscriptionMode.AsyncEnumerable;
                this.subscriptionTask = Task.Run(() => WatchForEvents((IAsyncEnumerable<TEventType>)subscriptionResult));
            }
        }

        private async void WatchForEvents(IAsyncEnumerable<TEventType> subscriptionResult)
        {
            try
            {
                await foreach (var item in subscriptionResult)
                {
                    await SendNewData(item);
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private async Task SendNewData(TEventType item)
        {
            var data = subscriptionStatement.ExecuteSubscriptionEvent(subscriptionNode, item);
            var result = new QueryResult();
            result.SetData(new Dictionary<string, object?> { { subscriptionNode.Name, data } });
            await server.SendNextAsync(id, result);
        }

        public void OnNext(TEventType item)
        {
            try
            {
                SendNewData(item).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void OnError(Exception error)
        {
            server.SendErrorAsync(id, error).GetAwaiter().GetResult();
        }

        public void OnCompleted()
        {
            server.CompleteSubscription(id);
        }

        public void Dispose()
        {
            if (mode == WebSocketSubscriptionMode.Observable)
            {
                subscription!.Dispose();
            }
            else
            {

            }
        }
    }

    internal enum WebSocketSubscriptionMode
    {
        None,
        Observable,
        AsyncEnumerable
    }

    public interface IWebSocketSubscription : IDisposable
    {
    }
}