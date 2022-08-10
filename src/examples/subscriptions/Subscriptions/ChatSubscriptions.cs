using subscriptions.Services;
using EntityGraphQL.Schema;

namespace subscriptions.Subscriptions
{
    public class ChatSubscriptions
    {
        [GraphQLSubscription("Example of a subscription with IObservable<T>")]
        public IObservable<Message> OnMessage(ChatService chat)
        {
            return chat.Subscribe();
        }
        [GraphQLSubscription("Example of a subscription with IAsyncEnumerable<T>")]
        public IAsyncEnumerable<Message> OnMessageEnumerable(ChatServiceEnumerable chat)
        {
            return chat.Subscribe();
        }
    }
}