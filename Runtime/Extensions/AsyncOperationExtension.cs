using System.Threading.Tasks;
using Frame.Runtime.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Extensions
{
    public static class AsyncOperationExtension
    {
        /// <summary>
        /// Based on the plattform will automatically select the correct way of dealing with the tasks.
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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