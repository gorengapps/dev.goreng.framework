using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Frame.Runtime.Extensions
{
    public static class TaskExtensions
    {
        // for void Tasks
        public static Awaitable AsAwaitable(this Task task)
        {
            var src = new AwaitableCompletionSource();
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)   src.SetException(t.Exception!);
                    else if (t.IsCanceled) src.SetCanceled();
                    else                src.SetResult();
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.FromCurrentSynchronizationContext());
            return src.Awaitable;
        }

        // for Task<T>
        public static Awaitable<T> AsAwaitable<T>(this Task<T> task)
        {
            var src = new AwaitableCompletionSource<T>();
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)   src.SetException(t.Exception!);
                    else if (t.IsCanceled) src.SetCanceled();
                    else                src.SetResult(t.Result);
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.FromCurrentSynchronizationContext());
            return src.Awaitable;
        }
        
        public static async Task AsTask(this Awaitable a)
        {
            await a;
        }

        public static async Task<T> AsTask<T>(this Awaitable<T> a)
        {
            return await a;
        }
    }
}