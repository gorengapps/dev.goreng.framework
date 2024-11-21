using System.Threading.Tasks;
using Frame.Runtime.Scene;

namespace Frame.Runtime.Navigation
{
    public interface INavigationService
    {
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
        public Task<T> ShowScene<T>(string destination, bool setActive = false);
        
        /// <summary>
        /// Allows you to show a supplementary scene on top of your active scene, without getting a handle
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public Task ShowScene(string destination, bool setActive = false);

        /// <summary>
        /// Tries to fetch an existing handle if it exists
        /// </summary>
        /// <param name="type">The type of scene handle we want</param>
        /// <typeparam name="T">Type of the scene</typeparam>
        /// <returns></returns>
        public T GetSceneHandle<T>(string type) where T: class;
        
        /// <summary>
        /// Unloads the given scene handle
        /// </summary>
        /// <param name="sceneHandle">The handle to the actual scene</param>
        /// <returns></returns>
        public Task Unload(IAsyncScene sceneHandle);
    }
}