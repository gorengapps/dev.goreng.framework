using System.Threading.Tasks;
using Frame.Runtime.Scene;

namespace Frame.Runtime.Navigation
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigate from one scene to the next
        /// </summary>
        /// <param name="destination">The scene that we want to load</param>
        /// <param name="intermediate">The scene we want to use as intermediate</param>
        /// <returns></returns>
        public Task NavigateTo(string destination, string intermediate);

        /// <summary>
        /// Navigates to a scene without showing a loading screen
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task Navigate(string destination);

        /// <summary>
        /// Allows you to show a supplementary scene on top of your active scene 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public Task<T> ShowSupplementaryScene<T>(string destination, bool setActive);

        /// <summary>
        /// Tries to fetch an existing handle if it exists
        /// </summary>
        /// <param name="type">The type of scene handle we want</param>
        /// <typeparam name="T">Type of the scene</typeparam>
        /// <returns></returns>
        public T GetSupplementarySceneHandle<T>(string type) where T: class;
        
        /// <summary>
        /// Unloads the given scene handle
        /// </summary>
        /// <param name="sceneHandle">The handle to the actual scene</param>
        /// <returns></returns>
        public Task Unload(IAsyncScene sceneHandle);
    }
}