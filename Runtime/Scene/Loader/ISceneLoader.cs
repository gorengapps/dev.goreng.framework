using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene.Loader
{
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads the given scene using an asset reference
        /// </summary>
        /// <param name="assetReference">The asset that is the to be loaded scene</param>
        /// <param name="mode">Loading mode of the scene</param>
        /// <returns></returns>
        public Awaitable<UnityEngine.SceneManagement.Scene> LoadScene(AssetReference assetReference, LoadSceneMode mode = LoadSceneMode.Additive);
    }
}