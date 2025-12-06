using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frame.Runtime.Data
{
    /// <summary>
    /// component responsible for releasing an addressable asset when the game object is destroyed.
    /// Used by DataService.LoadAndInstantiateAsync to prevent memory leaks.
    /// </summary>
    public class AddressableReleaser : MonoBehaviour
    {
        private object _asset;
        private bool _released;

        /// <summary>
        /// Initializes the releaser with the asset to release.
        /// </summary>
        /// <param name="asset">The addressable asset to release.</param>
        public void Initialize(object asset)
        {
            _asset = asset;
        }

        private void OnDestroy()
        {
            if (_released || _asset == null)
            {
                return;
            }

            Addressables.Release(_asset);
            _asset = null;
            _released = true;
        }
    }
}
