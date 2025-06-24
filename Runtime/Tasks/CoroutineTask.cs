using System;
using System.Collections;
using System.Threading.Tasks;
using Framework.Loop;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Tasks
{
    /// <summary>
    /// Abstract base class for coroutine-based task execution.
    /// Provides infrastructure for running async operations using Unity's coroutine system,
    /// particularly useful for WebGL platform compatibility.
    /// </summary>
    public abstract class CoroutineTask
    {
        /// <summary>
        /// The run loop instance used for executing coroutines.
        /// </summary>
        protected static IRunLoop _runLoop;
        
        /// <summary>
        /// Sets the run loop instance to be used for coroutine execution.
        /// This must be called during framework initialization.
        /// </summary>
        /// <param name="runLoop">The run loop instance to use for coroutine execution.</param>
        public static void SetRunLoop(IRunLoop runLoop)
        {
            _runLoop = runLoop;
        }
    }
    
    /// <summary>
    /// Generic coroutine task implementation that wraps Unity's AsyncOperationHandle&lt;T&gt;
    /// and provides Task-based interface for async/await operations.
    /// Automatically handles operation completion, failure, and exception scenarios.
    /// </summary>
    /// <typeparam name="T">The type of result expected from the async operation.</typeparam>
    public class CoroutineTask<T>: CoroutineTask
    {
        /// <summary>
        /// The async operation handle being wrapped.
        /// </summary>
        private readonly AsyncOperationHandle<T> _handle;
        
        /// <summary>
        /// Task completion source for bridging between coroutine and Task-based async patterns.
        /// </summary>
        private readonly TaskCompletionSource<T> _completed = new TaskCompletionSource<T>();
        
        /// <summary>
        /// Internal coroutine that polls the async operation until completion.
        /// </summary>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator RunCoroutine()
        {
            while (!_handle.IsDone)
            {
                yield return null;
            }
            
            if(_handle.Status == AsyncOperationStatus.Succeeded)
            {
                _completed.SetResult(_handle.Result);
            }
            else if (_handle.Status == AsyncOperationStatus.Failed)
            {
                _completed.SetException(_handle.OperationException ?? new InvalidOperationException("Operation failed with unknown error"));
            }
            else
            {
                // Handle None or other unexpected states
                _completed.SetException(new InvalidOperationException($"Operation completed with unexpected status: {_handle.Status}"));
            }
        }
        
        /// <summary>
        /// Initializes a new instance of CoroutineTask with the specified async operation handle.
        /// </summary>
        /// <param name="handle">The async operation handle to wrap. Must be valid.</param>
        /// <exception cref="InvalidOperationException">Thrown when the provided handle is invalid.</exception>
        public CoroutineTask(AsyncOperationHandle<T> handle)
        {
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Operation handle is invalid");
            }
            
            _handle = handle;
        }

        /// <summary>
        /// Starts the coroutine-based async operation and returns a Task that can be awaited.
        /// </summary>
        /// <returns>A Task&lt;T&gt; that represents the asynchronous operation and its result.</returns>
        public async Task<T> RunAsync()
        {
            _runLoop.Coroutine(RunCoroutine());
            return await _completed.Task;
        }
    }
}