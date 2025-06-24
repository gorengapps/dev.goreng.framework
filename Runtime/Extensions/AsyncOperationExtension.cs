using System.Threading.Tasks;
using Frame.Runtime.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Extensions
{
    /// <summary>
    /// Extension methods for Unity's AsyncOperationHandle types.
    /// Provides platform-specific Task conversion with automatic fallback to coroutine-based execution on WebGL.
    /// </summary>
    public static class AsyncOperationExtension
    {
        /// <summary>
        /// Converts an AsyncOperationHandle&lt;T&gt; to a Task&lt;T&gt; with platform-specific optimizations.
        /// On WebGL platforms, uses coroutine-based execution for compatibility.
        /// On other platforms, uses the native Task property for better performance.
        /// </summary>
        /// <typeparam name="T">The type of the async operation result.</typeparam>
        /// <param name="task">The AsyncOperationHandle to convert.</param>
        /// <returns>A Task&lt;T&gt; that represents the asynchronous operation.</returns>
        public static Task<T> ToTask<T>(this AsyncOperationHandle<T> task) 
        {
            #if UNITY_WEBGL
                return new CoroutineTask<T>(task).RunAsync();
            #else
                return task.Task;
            #endif
        }
    }
}