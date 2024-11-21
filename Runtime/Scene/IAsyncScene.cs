using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Scene.Loader;
using Framework.Loop;

namespace Frame.Runtime.Scene
{
    public interface IAsyncScene
    {
        /// <summary>
        /// Loader that will be used
        /// </summary>
        internal static ISceneLoader loader = new DefaultSceneLoader();
        
        /// <summary>
        /// name of the scene
        /// </summary>
        public string sceneType { get; }
        
        /// <summary>
        /// The associated scene
        /// </summary>
        public UnityEngine.SceneManagement.Scene  associatedScene { get; }

        /// <summary>
        /// Method allows you to replace the default loading behaviour
        /// </summary>
        /// <param name="loader">The scene loader that allows for scene loading</param>
        public static void SetSceneLoader(ISceneLoader sceneLoader)
        {
            IAsyncScene.loader = sceneLoader;
        }

        /// <summary>
        /// Called before the scene will be unloaded
        /// </summary>
        public Task SceneWillUnload();
        
        /// <summary>
        /// Loads the scene and activates it immediately
        /// </summary>
        public Task<IBootstrap> Load(bool setActive = true);
        
        /// <summary>
        /// Unloads a scene
        /// </summary>
        public Task Unload();
    }
}