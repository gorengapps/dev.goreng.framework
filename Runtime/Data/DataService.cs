using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Data
{
    public class DataService : IDataService
    {
        public async Task<List<T>> LoadList<T>(string key)
        {
            return await LoadAssetsAsync<T>(key);
        }

        public async Task<T> LoadAssetAsync<T>(string key)
        {
            T asset = default;

            try
            {
                // Load the asset asynchronously using the provided key
                var handle = Addressables.LoadAssetAsync<T>(key);

                // Await the completion of the asset loading
                asset = await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load asset with key '{key}'.");
                    return default;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception while loading asset with key '{key}': {ex.Message}");
            }
            
            return asset;
        }

        public async Task<T> LoadAndInstantiateAsync<T>(string key) where T: class
        {
            try
            {
                var asset = await LoadAssetAsync<GameObject>(key);

                // Instantiate the loaded asset
                var instance = Object.Instantiate(asset);

                if (instance.TryGetComponent(typeof(T), out var component))
                {
                    return component as T;
                }

                Debug.LogError($"Component of type '{typeof(T)}' not found on the instantiated object.");
                Object.Destroy(instance);
                return default;

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception while loading and instantiating asset with key '{key}': {ex.Message}");
                return default;
            }
        }

        private async Task<List<T>> LoadAssetsAsync<T>(string key)
        {
            // Load the resource locations matching the key and type T
            var locationsHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            var locations = await locationsHandle.Task;

            // Check if any locations were found
            if (locations == null || locations.Count == 0)
            {
                Addressables.Release(locationsHandle);
                Debug.LogWarning($"No resource locations found for key '{key}' and type '{typeof(T)}'.");
                return new List<T>();
            }

            var assets = new List<T>();
            var assetHandles = locations.Select(Addressables.LoadAssetAsync<T>).ToList();
            
            // Load each asset asynchronously

            // Wait for all assets to load
            await Task.WhenAll(assetHandles.Select(handle => handle.Task));

            // Collect loaded assets and handle any failures
            foreach (var handle in assetHandles)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    assets.Add(handle.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load asset at location '{handle.DebugName}'.");
                }
            }
            return assets;
        }
    }
}