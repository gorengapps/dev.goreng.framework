using System.Threading.Tasks;

namespace Frame.Runtime.Canvas
{
    public interface ICanvas
    {
        /// <summary>
        /// Signals that the scene will be unloaded in the near future
        /// </summary>
        public void SceneWillUnload();

        /// <summary>
        /// Signals that the scene will be loaded
        /// </summary>
        public Task SceneWillLoad();
    }
}