using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.DI.Provider;

namespace Frame.Runtime.Canvas
{
    public interface ICanvas
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
            ICanvas.provider = provider;
        }
        
        /// <summary>
        /// Signals that the scene will be unloaded in the near future
        /// </summary>
        public Task SceneWillUnload();

        /// <summary>
        /// Signals that the scene will be loaded
        /// </summary>
        public Task SceneWillLoad();
    }
}