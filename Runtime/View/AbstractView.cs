using Framework.Events;
using UnityEngine;

namespace Frame.Runtime.Canvas
{
    /// <summary>
    /// Abstract base class for view components within the Frame.Runtime framework.
    /// Provides default implementations for view lifecycle methods and manages
    /// event subscriptions through a disposal bag pattern.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to create custom view implementations that automatically
    /// handle Unity MonoBehaviour lifecycle and provide clean subscription management.
    /// </remarks>
    public class AbstractView: MonoBehaviour, IView
    {
        /// <summary>
        /// Disposal bag for managing event subscriptions and other disposable resources.
        /// All subscriptions added to this bag will be automatically disposed when the view unloads.
        /// </summary>
        protected readonly DisposeBag _disposeBag = new DisposeBag();
        
        /// <summary>
        /// Called when the view is about to be unloaded.
        /// Override this method to implement custom cleanup logic.
        /// The base implementation disposes all subscriptions in the disposal bag.
        /// </summary>
        /// <returns>An awaitable that completes when the unload operation is finished.</returns>
        public virtual Awaitable ViewWillUnloadAsync()
        {
            _disposeBag.Dispose();
            return Awaitable.EndOfFrameAsync();
        }

        /// <summary>
        /// Called when the view is about to be loaded.
        /// Override this method to implement custom initialization logic.
        /// </summary>
        /// <returns>An awaitable that completes when the load operation is finished.</returns>
        public virtual Awaitable ViewWillLoadAsync()
        {
            return Awaitable.EndOfFrameAsync();
        }
    }
}