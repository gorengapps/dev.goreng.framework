using Frame.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene.Loader
{
    internal class DefaultSceneLoader: ISceneLoader
    {
        public async Awaitable<UnityEngine.SceneManagement.Scene> LoadScene(AssetReference assetReference, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var result = await assetReference
                .LoadSceneAsync(mode)
                .ToTask();
            
            return result.Scene;
        }
    }
}