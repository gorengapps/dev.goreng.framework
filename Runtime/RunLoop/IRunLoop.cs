using System;
using System.Collections;

namespace Frame.Runtime.RunLoop
{
    public interface IRunLoop
    {
        /// <summary>
        /// Subscribe to runloop notifications
        /// </summary>
        /// <param name="callback"></param>
        public void Subscribe(Action<float> callback);
        
        /// <summary>
        /// Unsubscribe from runloop notifications
        /// </summary>
        /// <param name="callback"></param>
        public void UnSubscribe(Action<float> callback);

        /// <summary>
        /// Starts a routine
        /// </summary>
        /// <param name="routine"></param>
        public void Coroutine(IEnumerator routine);
        
        /// <summary>
        /// Stops a routine
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine(IEnumerator routine);
    }
}