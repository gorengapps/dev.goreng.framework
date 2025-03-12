using System;
using System.Collections;
using System.Threading.Tasks;
using Framework.Loop;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Tasks
{
    public abstract class CoroutineTask
    {
        protected static IRunLoop _runLoop;
        
        public static void SetRunLoop(IRunLoop runLoop)
        {
            _runLoop = runLoop;
        }
    }
    
    public class CoroutineTask<T>: CoroutineTask
    {
        private readonly AsyncOperationHandle<T> _handle;
        private readonly TaskCompletionSource<T> _completed = new TaskCompletionSource<T>();
        
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

            if (_handle.Status is AsyncOperationStatus.Failed or AsyncOperationStatus.None)
            {
                _completed.SetException(_handle.OperationException);
            }
        }
        
        public CoroutineTask(AsyncOperationHandle<T> handle)
        {
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Operation handle is invalid");
            }
            
            _handle = handle;
        }

        public async Task<T> RunAsync()
        {
            _runLoop.Coroutine(RunCoroutine());
            return await _completed.Task;
        }
    }
}