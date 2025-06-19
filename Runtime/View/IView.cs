using Framework.DI.Provider;
using UnityEngine;

namespace Frame.Runtime.Canvas
{
    /// <summary>
    /// Defines the contract for canvas components within a scene.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Static provider to resolve internal dependencies
        /// </summary>
        protected static IDependencyProvider provider;
        
        /// <summary>
        /// Binds a dependency provider to the bootstrap
        /// </summary>
        /// <param name="provider">The provider that we want to use to resolve dependencies</param>
        static void SetProvider(IDependencyProvider provider)
        {
            IView.provider = provider;
        }
        
        /// <summary>
        /// Signals that the scene will be unloaded in the near future.
        /// Perform any necessary cleanup operations here.
        /// </summary>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        Awaitable ViewWillUnloadAsync();

        /// <summary>
        /// Signals that the scene will be loaded.
        /// Perform any necessary initialization operations here.
        /// </summary>
        /// <returns>A task that represents the asynchronous load operation.</returns>
        Awaitable ViewWillLoadAsync();
    }
}