using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Frame.Runtime.Extensions
{
    /// <summary>
    /// Extension methods for converting between Unity's Awaitable and .NET's Task types.
    /// Provides seamless interoperability between different asynchronous programming models.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Converts a .NET Task to Unity's Awaitable.
        /// Preserves exception handling and cancellation status during conversion.
        /// </summary>
        /// <param name="task">The Task to convert.</param>
        /// <returns>An Awaitable that represents the same asynchronous operation.</returns>
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

        /// <summary>
        /// Converts a .NET Task&lt;T&gt; to Unity's Awaitable&lt;T&gt;.
        /// Preserves the result value, exception handling, and cancellation status during conversion.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The Task&lt;T&gt; to convert.</param>
        /// <returns>An Awaitable&lt;T&gt; that represents the same asynchronous operation.</returns>
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
        
        /// <summary>
        /// Converts Unity's Awaitable to a .NET Task.
        /// Enables compatibility with libraries and APIs that expect Task types.
        /// </summary>
        /// <param name="a">The Awaitable to convert.</param>
        /// <returns>A Task that represents the same asynchronous operation.</returns>
        public static async Task AsTask(this Awaitable a)
        {
            await a;
        }

        /// <summary>
        /// Converts Unity's Awaitable&lt;T&gt; to a .NET Task&lt;T&gt;.
        /// Enables compatibility with libraries and APIs that expect Task&lt;T&gt; types.
        /// </summary>
        /// <typeparam name="T">The type of the awaitable result.</typeparam>
        /// <param name="a">The Awaitable&lt;T&gt; to convert.</param>
        /// <returns>A Task&lt;T&gt; that represents the same asynchronous operation.</returns>
        public static async Task<T> AsTask<T>(this Awaitable<T> a)
        {
            return await a;
        }
    }
}