using System.Threading.Tasks;
using Frame.Runtime.Canvas;
using Frame.Runtime.Scene;
using Framework.DI.Provider;

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
        /// The bootstrap call that will be run after the scene has been loaded
        /// </summary>
        void OnBootstrapStart();
        
        /// <summary>
        /// The bootstrap call that will be run before the scene will unload
        /// </summary>
        void OnBootstrapStop();

        /// <summary>
        /// Unloads the current scene
        /// </summary>
        /// <returns></returns>
        Task Unload();

        /// <summary>
        /// Fetch an active canvas that lives in this scene scope
        /// </summary>
        /// <typeparam name="T">The canvas we want to fetch</typeparam>
        /// <returns>The canvas type if found</returns>
        T FetchCanvas<T>() where T : ICanvas;
        
        /// <summary>
        /// Scene will unload in the near future 
        /// </summary>
        Task SceneWillUnload();
        
        /// <summary>
        /// Scene will load in the near future 
        /// </summary>
        Task SceneWillLoad();
        
        /// <summary>
        /// Called whenever the scene is done loading and the bootstrap can be run
        /// </summary>
        /// <param name="sceneContext"></param>
        void Load(IAsyncScene sceneContext);
    }
}