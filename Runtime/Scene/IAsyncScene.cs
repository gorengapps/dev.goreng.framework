using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Scene.Loader;

namespace Frame.Runtime.Scene
{
    public interface IAsyncScene
    {
        /// <summary>
        /// Loader that will be used
        /// </summary>
        internal static ISceneLoader loader = new DefaultSceneLoader();
        
        /// <summary>
        /// Updates the ISceneLoader that will be used internally
        /// </summary>
        /// <param name="sceneLoader"></param>
        public static void SetSceneLoader(ISceneLoader sceneLoader)
        {
            IAsyncScene.loader = sceneLoader;
        }
        
        /// <summary>
        /// Gets the type identifier of the scene.
        /// </summary>
        public string sceneType { get; }

        /// <summary>
        /// Gets the associated Unity scene.
        /// </summary>
        public UnityEngine.SceneManagement.Scene associatedScene { get; }

        /// <summary>
        /// Called before the scene will be unloaded to perform any necessary cleanup.
        /// </summary>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        public Task SceneWillUnloadAsync();

        /// <summary>
        /// Loads the scene asynchronously and optionally sets it as the active scene.
        /// </summary>
        /// <param name="setActive">If set to <c>true</c>, the loaded scene will be set as the active scene.</param>
        /// <returns>
        /// A task that represents the asynchronous load operation.
        /// The task result contains the bootstrap instance associated with the loaded scene.
        /// </returns>
        public Task<IBootstrap> LoadAsync(bool setActive = true);

        /// <summary>
        /// Unloads the scene asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        public Task UnloadAsync();
    }
}