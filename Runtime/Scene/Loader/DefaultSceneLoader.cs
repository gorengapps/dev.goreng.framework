using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene.Loader
{
    internal class DefaultSceneLoader: ISceneLoader
    {
        public async Task<UnityEngine.SceneManagement.Scene> LoadScene(AssetReference assetReference, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var result = await assetReference
                .LoadSceneAsync(mode)
                .Task;
            
            return result.Scene;
        }
    }
}