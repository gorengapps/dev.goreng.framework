using Framework.Events;
using UnityEngine;

namespace Frame.Runtime.Canvas
{
    public class AbstractView: MonoBehaviour, IView
    {
        /// <summary>
        /// Can hold relevant subscriptions
        /// </summary>
        protected readonly DisposeBag _disposeBag = new DisposeBag();
        
        public virtual Awaitable ViewWillUnloadAsync()
        {
            _disposeBag.Dispose();
            return Awaitable.EndOfFrameAsync();
        }

        public virtual Awaitable ViewWillLoadAsync()
        {
            return Awaitable.EndOfFrameAsync();
        }
    }
}