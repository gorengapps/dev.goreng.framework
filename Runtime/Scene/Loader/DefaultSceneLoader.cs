using Frame.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene.Loader
{
    /// <summary>
    /// Default implementation of ISceneLoader interface.
    /// Provides scene loading functionality using Unity's Addressable Asset System.
    /// This loader is used internally by the framework for standard scene loading operations.
    /// </summary>
    internal class DefaultSceneLoader: ISceneLoader
    {
        /// <summary>
        /// Loads a scene asynchronously using the specified asset reference and load mode.
        /// </summary>
        /// <param name="assetReference">The addressable asset reference pointing to the scene to load.</param>
        /// <param name="mode">The loading mode for the scene (Additive by default).</param>
        /// <returns>An Awaitable that represents the asynchronous operation. The result contains the loaded Unity scene.</returns>
        public async Awaitable<UnityEngine.SceneManagement.Scene> LoadScene(AssetReference assetReference, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var result = await assetReference
                .LoadSceneAsync(mode)
                .ToTask();
            
            return result.Scene;
        }
    }
}