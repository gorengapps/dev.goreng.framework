using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Scene;

namespace Frame.Runtime.Navigation
{
    /// <summary>
    /// Defines methods for scene navigation, including loading, showing, and unloading scenes.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Initialises the navigation service
        /// </summary>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        Task Initialise();
        
        /// <summary>
        /// Navigates to a specified scene without displaying a loading screen.
        /// </summary>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        Task NavigateAsync<T>() where T: class, IBootstrap;

        /// <summary>
        /// Shows a supplementary scene on top of the active scene and returns a handle to its bootstrap instance.
        /// </summary>
        /// <typeparam name="T">The type of the bootstrap instance, which must implement <see cref="IBootstrap"/>.</typeparam>
        /// <param name="setActive">Indicates whether to set the loaded scene as the active scene. Defaults to <c>false</c>.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the bootstrap instance of type <typeparamref name="T"/> for the loaded scene.
        /// </returns>
        Task<T> ShowSceneAsync<T>(bool setActive = false) where T : class, IBootstrap;
        
        /// <summary>
        /// Attempts to retrieve an existing bootstrap instance of a loaded scene.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bootstrap instance to retrieve, which must implement <see cref="IBootstrap"/>.
        /// </typeparam>
        /// <returns>
        /// The bootstrap instance of type <typeparamref name="T"/> if it exists; otherwise, <c>null</c>.
        /// </returns>
        T GetSceneHandle<T>() where T : class, IBootstrap;

        /// <summary>
        /// Unloads a specified scene using its asynchronous scene handle.
        /// </summary>
        /// <param name="sceneHandle">The handle to the scene to unload, implementing <see cref="IAsyncScene"/>.</param>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        Task UnloadAsync(IAsyncScene sceneHandle);
    }
}