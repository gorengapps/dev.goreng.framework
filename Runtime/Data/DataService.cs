using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame.Runtime.Data
{
    /// <summary>
    /// Default implementation of the IDataService interface.
    /// Provides asset loading functionality using Unity's Addressable Asset System.
    /// Supports loading single assets, lists of assets, and instantiated assets with component retrieval.
    /// </summary>
    [UsedImplicitly]
    public class DataService : IDataService
    {
        /// <summary>
        /// Loads a list of assets of the specified type using the provided key.
        /// </summary>
        /// <typeparam name="T">The type of assets to load.</typeparam>
        /// <param name="key">The addressable key used to locate the assets.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of loaded assets.</returns>
        public async Awaitable<List<T>> LoadListAsync<T>(string key)
        {
            return await LoadAssetsAsync<T>(key);
        }
        
        /// <summary>
        /// Loads a list of GameObjects and extracts components of the specified type from them.
        /// </summary>
        /// <typeparam name="T">The component type to extract from the loaded GameObjects.</typeparam>
        /// <param name="key">The addressable key used to locate the GameObject assets.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of components of type T.</returns>
        public async Awaitable<List<T>> LoadListAsyncAs<T>(string key)
        {
            var list = await LoadListAsync<GameObject>(key);
            
            return list
                .Select(gameObject => gameObject.GetComponent<T>())
                .Where(x => x != null)
                .ToList();
        }

        /// <summary>
        /// Loads a single asset of the specified type using the provided key.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="key">The addressable key used to locate the asset.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded asset, or default(T) if loading fails.</returns>
        public async Awaitable<T> LoadAssetAsync<T>(string key)
        {
            T asset = default;

            try
            {
                // Load the asset asynchronously using the provided key
                var handle = Addressables.LoadAssetAsync<T>(key);

                // Await the completion of the asset loading
                asset = await handle.ToTask();

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
        
        /// <summary>
        /// Loads a GameObject asset and extracts a component of the specified type from it.
        /// </summary>
        /// <typeparam name="T">The component type to extract from the loaded GameObject.</typeparam>
        /// <param name="key">The addressable key used to locate the GameObject asset.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the component of type T, or null if not found.</returns>
        public async Awaitable<T> LoadAssetAsyncAs<T>(string key) where T: class
        {
            var asset = await LoadAssetAsync<GameObject>(key);
            
            if (asset.TryGetComponent(typeof(T), out var component))
            {
                return component as T;
            }
            
            return default;
        }

        /// <summary>
        /// Loads a GameObject asset, instantiates it, and extracts a component of the specified type.
        /// If the component is not found or an error occurs, the instantiated object is destroyed.
        /// </summary>
        /// <typeparam name="T">The component type to extract from the instantiated GameObject.</typeparam>
        /// <param name="key">The addressable key used to locate the GameObject asset.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the component of type T, or null if instantiation or component retrieval fails.</returns>
        public async Awaitable<T> LoadAndInstantiateAsync<T>(string key) where T: class
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

        /// <summary>
        /// Internal method that handles loading multiple assets using Addressable resource locations.
        /// Loads all assets matching the specified key and type, handling failures gracefully.
        /// </summary>
        /// <typeparam name="T">The type of assets to load.</typeparam>
        /// <param name="key">The addressable key used to locate the assets.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of successfully loaded assets.</returns>
        private async Awaitable<List<T>> LoadAssetsAsync<T>(string key)
        {
            // Load the resource locations matching the key and type T
            var locationsHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            var locations = await locationsHandle.ToTask();

            // Check if any locations were found
            if (locations == null || locations.Count == 0)
            {
                Addressables.Release(locationsHandle);
                Debug.LogWarning($"No resource locations found for key '{key}' and type '{typeof(T)}'.");
                return new List<T>();
            }

            var assets = new List<T>();
            var assetHandles = locations
                .Select(Addressables.LoadAssetAsync<T>)
                .ToList();

            // Wait for all assets to load
            var task = Task.WhenAll(assetHandles.Select(handle => handle.ToTask()));

            await task.AsAwaitable();
            
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