using System.Collections.Generic;
using UnityEngine;

namespace Frame.Runtime.Data
{
    public interface IDataService
    {
        /// <summary>
        /// Loads a list of assets of type T associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of assets to load.</typeparam>
        /// <param name="key">The key associated with the assets to load.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of loaded assets of type T.
        /// </returns>
        Awaitable<List<T>> LoadList<T>(string key);
        
        /// <summary>
        /// Loads a list of assets of type T associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of assets to load.</typeparam>
        /// <param name="key">The key associated with the assets to load.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of loaded assets of type T.
        /// </returns>
        Awaitable<List<T>> LoadListAs<T>(string key);

        /// <summary>
        /// Loads a single asset of type T associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the asset to load.</typeparam>
        /// <param name="key">The key associated with the asset to load.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the loaded asset of type T.
        /// </returns>
        Awaitable<T> LoadAssetAsync<T>(string key);

        /// <summary>
        /// Loads a single asset of type T associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the asset to load.</typeparam>
        /// <param name="key">The key associated with the asset to load.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the loaded asset of type T.
        /// </returns>
        Awaitable<T> LoadAssetAsyncAs<T>(string key) where T : class;

        /// <summary>
        /// Asynchronously loads an asset by key, instantiates it, and retrieves a component of type T from the instantiated GameObject.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the component to retrieve from the instantiated asset.
        /// </typeparam>
        /// <param name="key">The key of the asset to load and instantiate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the component of type T from the instantiated asset.
        /// Returns <c>default</c> if the asset fails to load, instantiate, or does not contain the component.
        /// </returns>
        Awaitable<T> LoadAndInstantiateAsync<T>(string key) where T: class;
    }
}