using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EntityGraphQL.Subscriptions;

public class SubscriptionBroadcaster<TType>
{
    private readonly List<SubscriptionEnumerable<TType>> subsciprtionEnumerables = new();
    public IAsyncEnumerable<TType> NewSubscription()
    {
        var s = new SubscriptionEnumerable<TType>();
        subsciprtionEnumerables.Add(s);
        return s;
    }

    public void OnNext(TType msg)
    {
        foreach (var item in subsciprtionEnumerables)
        {
            item.PushNext(msg);
        }
    }
}

public class SubscriptionEnumerable<T> : IAsyncEnumerable<T>
{
    private TaskCompletionSource<T> tcs = new();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator(this);
    }

    public void PushNext(T msg)
    {
        tcs.SetResult(msg);
    }

    private sealed class Enumerator : IAsyncEnumerator<T>
    {
        readonly SubscriptionEnumerable<T> subscriptionEnumerable;
        private readonly CancellationToken cancellationToken;
        private bool cancel = false;

        public Enumerator(SubscriptionEnumerable<T> subscriptionEnumerable, CancellationToken cancellationToken = default)
        {
            this.subscriptionEnumerable = subscriptionEnumerable;
            this.cancellationToken = cancellationToken;
            Current = default!;
        }

        public T Current { get; private set; }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(MoveNextHelper());
        }

        private Task<bool> MoveNextHelper()
        {
            return subscriptionEnumerable.tcs.Task.ContinueWith(t =>
            {
                Current = default!;
                if (t.IsCanceled)
                    return false;
                if (t.IsFaulted)
                    throw t.Exception!;
                if (cancel)
                    return false;
                if (cancellationToken.IsCancellationRequested)
                {
                    cancel = true;
                    return false;
                }
                Current = t.Result;
                subscriptionEnumerable.tcs = new TaskCompletionSource<T>();
                return true;
            });
        }

        public ValueTask DisposeAsync()
        {
            cancel = true;
            return new ValueTask();
        }
    }
}
