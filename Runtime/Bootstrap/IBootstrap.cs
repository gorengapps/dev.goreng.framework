using System.Threading.Tasks;
using Frame.Runtime.Canvas;
using Frame.Runtime.Scene;
using Framework.DI.Provider;
using UnityEngine;

namespace Frame.Runtime.Bootstrap {
    public interface IBootstrap
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
            IBootstrap.provider = provider;
        }
        
        /// <summary>
        /// Called when the bootstrap starts after the scene has been loaded.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Awaitable OnBootstrapStartAsync();

        /// <summary>
        /// Called before the scene unloads to perform any necessary cleanup.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Awaitable OnBootstrapStopAsync();

        /// <summary>
        /// Unloads the current scene.
        /// </summary>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        Awaitable UnloadAsync();

        /// <summary>
        /// Fetches an active canvas that exists within the scene scope.
        /// </summary>
        /// <typeparam name="T">The type of canvas to fetch.</typeparam>
        /// <returns>The canvas instance if found; otherwise, <c>null</c>.</returns>
        T FetchView<T>() where T : IView;

        /// <summary>
        /// Called when the scene is about to unload in the near future.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Awaitable SceneWillUnloadAsync();

        /// <summary>
        /// Called when the scene is about to load in the near future.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Awaitable SceneWillLoadAsync();

        /// <summary>
        /// Called when the scene has finished loading, allowing the bootstrap to run.
        /// </summary>
        /// <param name="sceneContext">The context of the loaded scene.</param>
        void Load(IAsyncScene sceneContext);
    }
}