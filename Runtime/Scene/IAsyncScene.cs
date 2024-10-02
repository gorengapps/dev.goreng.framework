using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Framework.Loop;

namespace Frame.Runtime.Scene
{
    public interface IAsyncScene
    {
        public string sceneType { get; }
        
        public UnityEngine.SceneManagement.Scene  associatedScene { get; }
        
        /// <summary>
        /// Preloads the scene without activating it
        /// This is useful for scenes that want to fade in or fade out a loading screen 
        /// </summary>
        public Task Preload();

        /// <summary>
        /// Called before the scene will be unloaded
        /// </summary>
        public Task SceneWillUnload();
        
        /// <summary>
        /// Allows you to specify when the scene is done loading
        /// </summary>
        public Task WhenDone(IRunLoop runner);
        
        /// <summary>
        /// Continues the scene that was preloaded
        /// </summary>
        public Task Continue(IRunLoop runner);
        
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