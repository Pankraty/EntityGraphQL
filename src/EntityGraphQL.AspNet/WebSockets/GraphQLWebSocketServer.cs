using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EntityGraphQL.Subscriptions;

namespace EntityGraphQL.AspNet.WebScokets
{
    /// <summary>
    /// Implementation of the GraphQL over WebSocket protocol - https://github.com/enisdenjo/graphql-ws/blob/master/PROTOCOL.md.
    /// </summary>
    /// <typeparam name="TQueryType"></typeparam>
    public class GraphQLWebSocketServer<TQueryType> : IGraphQLWebSocketServer
    {
        private readonly Dictionary<Guid, IWebSocketSubscription> subscriptions = new Dictionary<Guid, IWebSocketSubscription>();
        private readonly WebSocket webSocket;
        private readonly HttpContext context;
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public GraphQLWebSocketServer(WebSocket webSocket, HttpContext context)
        {
            this.webSocket = webSocket;
            this.context = context;
        }

        public async Task HandleAsync()
        {
            while (!webSocket.CloseStatus.HasValue)
            {
                using var memoryStream = new MemoryStream();
                WebSocketReceiveResult? receiveResult = null;
                do
                {
                    var buffer = new byte[1024 * 4];
                    var segment = new ArraySegment<byte>(buffer);
                    receiveResult = await webSocket.ReceiveAsync(segment, CancellationToken.None);

                    if (receiveResult.CloseStatus.HasValue)
                    {
                        await CloseConnectionAsync(receiveResult.CloseStatus.Value, webSocket.CloseStatusDescription);
                        break;
                    }

                    if (receiveResult.Count == 0)
                        continue;

                    await memoryStream.WriteAsync(segment.Array!, segment.Offset, receiveResult.Count);
                } while (!receiveResult.EndOfMessage || memoryStream.Length == 0);

                if (!webSocket.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(memoryStream.ToArray());
                    await HandleMessageAsync(JsonSerializer.Deserialize<GraphQLWSRequest>(message, jsonOptions)!);
                }
            }

            await CloseConnectionAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription);
        }

        private async Task HandleMessageAsync(GraphQLWSRequest graphQLWSMessage)
        {
            switch (graphQLWSMessage.Type)
            {
                case GraphQLWSMessageType.CONNECTION_INIT: await SendSimpleResponseAsync(GraphQLWSMessageType.CONNECTION_ACK); break;
                case GraphQLWSMessageType.PING: await SendSimpleResponseAsync(GraphQLWSMessageType.PONG); break;
                case GraphQLWSMessageType.SUBSCRIBE: await HandleSubscribeAsync(graphQLWSMessage); break;
                case GraphQLWSMessageType.COMPLETE: CompleteSubscription(graphQLWSMessage.Id!.Value); break;
                case GraphQLWSMessageType.PONG: break; // can come to us but we don't care
                default:
                    await CloseConnectionAsync(WebSocketCloseStatus.InvalidMessageType, $"Unknown message type: {graphQLWSMessage.Type}");
                    break;
            }
        }

        private async Task HandleSubscribeAsync(GraphQLWSRequest graphQLWSMessage)
        {
            var request = graphQLWSMessage.Payload!;
            var schema = context.RequestServices.GetService<SchemaProvider<TQueryType>>();
            if (schema == null)
                throw new InvalidOperationException("No SchemaProvider<TQueryType> found in the service collection. Make sure you set up your Startup.ConfigureServices() to call AddGraphQLSchema<TQueryType>().");

            var schemaContext = context.RequestServices.GetService<TQueryType>();
            if (schemaContext == null)
                throw new InvalidOperationException("No schema context was found in the service collection. Make sure the TQueryType used with MapGraphQL<TQueryType>() is registered in the service collection.");

            // executing this sets up the observers etc. We don't return any data until we have an event
            var result = await schema.ExecuteRequestAsync(request, schemaContext, context.RequestServices, context.User, null)!;
            if (result.Errors != null)
            {
                await SendErrorAsync(graphQLWSMessage.Id!.Value, result.Errors);
                return;
            }

            // Wonder if there is a better way to figure this out? Spec says subscription can only have a single root field
            // so if there are no errors we must have a successful subscription method result
            var subscriptionData = result.Data!.Values.First() as SubscriptionResult;
            var wsSubscription = (IWebSocketSubscription)Activator.CreateInstance(typeof(WebSocketSubscription<>).MakeGenericType(subscriptionData!.EventType), graphQLWSMessage.Id!.Value, subscriptionData!.SubscriptionObservable, (IGraphQLWebSocketServer)this, subscriptionData!.SubscriptionStatement, subscriptionData!.Field)!;
            subscriptions.Add(graphQLWSMessage.Id!.Value, wsSubscription!);
        }

        public async Task SendErrorAsync(Guid id, Exception error)
        {
            await SendErrorAsync(id, new List<GraphQLError> { new GraphQLError(error.Message, null) });
        }
        public async Task SendErrorAsync(Guid id, IEnumerable<GraphQLError> errors)
        {
            await SendAsync(new GraphQLWSError
            {
                Id = id,
                Type = GraphQLWSMessageType.ERROR,
                Payload = errors.ToList(),
            });
        }

        public void CompleteSubscription(Guid id)
        {
            // TODO should we close the connection?
            // if (!webSocket.CloseStatus.HasValue)
            // {
            //     await SendAsync(new WithIdGraphQLWSResponse
            //     {
            //         Id = id,
            //         Type = GraphQLWSMessageType.COMPLETE,
            //     });
            //     await CloseConnectionAsync(WebSocketCloseStatus.NormalClosure, "Subscription finished");
            // }
            if (subscriptions.ContainsKey(id))
            {
                subscriptions[id].Dispose();
                subscriptions.Remove(id);
            }
        }

        public async Task SendNextAsync(Guid id, QueryResult obj)
        {
            await SendAsync(new GraphQLWSResponse
            {
                Id = id,
                Type = GraphQLWSMessageType.NEXT,
                Payload = obj,
            });
        }

        private async Task SendSimpleResponseAsync(string type)
        {
            await SendAsync(new TypeOnlyGraphQLWSResponse
            {
                Type = type
            });
        }

        private async Task SendAsync(object graphQLWSMessage)
        {
            var json = JsonSerializer.Serialize(graphQLWSMessage, jsonOptions);
            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);
            await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task CloseConnectionAsync(WebSocketCloseStatus closeStatus, string? closeStatusDescription)
        {
            if (webSocket.State != WebSocketState.Closed &&
                webSocket.State != WebSocketState.CloseSent &&
                webSocket.State != WebSocketState.Aborted)
            {
                if (closeStatus == WebSocketCloseStatus.NormalClosure)
                    await webSocket.CloseAsync(closeStatus, closeStatusDescription, CancellationToken.None);
                else
                    await webSocket.CloseOutputAsync(closeStatus, closeStatusDescription, CancellationToken.None);
            }

            foreach (var subscription in subscriptions.Values)
                subscription.Dispose();
        }
    }

    public interface IGraphQLWebSocketServer
    {
        void CompleteSubscription(Guid id);
        Task SendErrorAsync(Guid id, Exception error);
        Task SendNextAsync(Guid id, QueryResult result);
    }
}