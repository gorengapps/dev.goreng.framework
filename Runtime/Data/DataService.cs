using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Data
{
    public partial class DataService : IDataService
    {
        public async Task<List<T>> LoadList<T>(string key)
        {
            return await LoadAssetsAsync<T>(key);
        }

        public async Task<T> LoadAssetAsync<T>(string key)
        {
            T asset = default;
            AsyncOperationHandle<T> handle = default;

            try
            {
                // Load the asset asynchronously using the provided key
                handle = Addressables.LoadAssetAsync<T>(key);

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
            finally
            {
                // Release the handle to prevent memory leaks
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            return asset;
        }

        public async Task<T> LoadAndInstantiateAsync<T>(string key)
        {
            AsyncOperationHandle<GameObject> handle = default;

            try
            {
                // Load the GameObject asset asynchronously using the provided key
                handle = Addressables.LoadAssetAsync<GameObject>(key);
                var asset = await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load asset with key '{key}'.");
                    return default;
                }

                // Instantiate the loaded asset
                var instance = Object.Instantiate(asset);

                // Retrieve the component of type TComponent from the instantiated object
                var component = instance.GetComponent<T>();

                if (component == null)
                {
                    Debug.LogError($"Component of type '{typeof(T)}' not found on the instantiated object.");
                    // Optionally, destroy the instantiated object if the component is not found
                    Object.Destroy(instance);
                    return default;
                }

                return component;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception while loading and instantiating asset with key '{key}': {ex.Message}");
                return default;
            }
            finally
            {
                // Release the handle to prevent memory leaks
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
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
            var assetHandles = new List<AsyncOperationHandle<T>>();

            try
            {
                // Load each asset asynchronously
                foreach (var location in locations)
                {
                    var assetHandle = Addressables.LoadAssetAsync<T>(location);
                    assetHandles.Add(assetHandle);
                }

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
            }
            finally
            {
                // Release all asset handles to prevent memory leaks
                foreach (var handle in assetHandles)
                {
                    if (handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }
                }

                // Release the locations handle
                Addressables.Release(locationsHandle);
            }

            return assets;
        }
    }
}