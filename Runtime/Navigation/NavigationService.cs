using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Frame.Runtime.Attributes;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Data;
using Frame.Runtime.Scene;
using JetBrains.Annotations;
using UnityEngine;

namespace Frame.Runtime.Navigation
{
    /// <summary>
    /// Default implementation of the INavigationService interface.
    /// Manages scene loading, unloading, and navigation operations using Unity's Addressable Asset System.
    /// Maintains a registry of scene mappings and tracks active bootstrap instances.
    /// </summary>
    [UsedImplicitly]
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// The default key used to load scene assets from the addressable system.
        /// </summary>
        private const string _scenesKey = "scenes";

        /// <summary>
        /// Maps bootstrap types to their corresponding scene instances.
        /// </summary>
        private readonly Dictionary<Type, IAsyncScene> _scenes = new Dictionary<Type, IAsyncScene>();
        
        /// <summary>
        /// Tracks currently loaded bootstrap instances by their type.
        /// </summary>
        private readonly Dictionary<Type, IBootstrap> _openScreens = new Dictionary<Type, IBootstrap>();
        
        /// <summary>
        /// Maps scene type names to their corresponding bootstrap types.
        /// </summary>
        private Dictionary<string, Type> _sceneMapping = new Dictionary<string, Type>();

        /// <summary>
        /// The data service used for loading scene assets.
        /// </summary>
        private readonly IDataService _dataService;
        
        /// <summary>
        /// Task representing the ongoing initialization operation, if any.
        /// </summary>
        private Awaitable _initialiseTask;
        
        /// <summary>
        /// Lock object to ensure thread-safe initialization.
        /// </summary>
        private readonly object _initialiseLock = new object();

        /// <summary>
        /// Initializes a new instance of the NavigationService with the specified data service.
        /// </summary>
        /// <param name="dataService">The data service to use for loading scene assets.</param>
        public NavigationService(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Initializes the navigation service with the provided scene mapping.
        /// This method is idempotent and will return the same task if called multiple times.
        /// </summary>
        /// <param name="sceneMapping">Dictionary mapping scene type names to bootstrap types.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public Awaitable Initialise(Dictionary<string, Type> sceneMapping)
        {
            if (_initialiseTask != null)
            {
                return _initialiseTask;
            }

            lock (_initialiseLock)
            {
                _initialiseTask = InitialiseInternal(sceneMapping);
            }

            return _initialiseTask;
        }

        /// <summary>
        /// Internal implementation of the initialization logic.
        /// Loads all scene assets and creates the scene registry.
        /// </summary>
        /// <param name="sceneMapping">Dictionary mapping scene type names to bootstrap types.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        private async Awaitable InitialiseInternal(Dictionary<string, Type> sceneMapping)
        {
            var scenesList = await _dataService.LoadListAsync<IAsyncScene>(_scenesKey);
            
            if (scenesList == null)
            {
                Debug.LogError("Failed to load scenes from data service.");
                return;
            }

            _sceneMapping = sceneMapping;
            _scenes.Clear();
            
            foreach (var scene in scenesList.Where(scene => scene != null && !string.IsNullOrEmpty(scene.sceneType)))
            {
                if (sceneMapping.TryGetValue(scene.sceneType, out var bootstrap))
                {
                    _scenes[bootstrap] = scene;
                }
                else
                {
                    Debug.LogError($"Scene '{scene.sceneType}' is not mapped in the scene mapping.");
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve a scene instance for the specified bootstrap type.
        /// </summary>
        /// <typeparam name="T">The bootstrap type associated with the scene.</typeparam>
        /// <param name="scene">The scene instance if found, otherwise null.</param>
        /// <returns>True if the scene was found, otherwise false.</returns>
        private bool TryGetScene<T>(out IAsyncScene scene)
        {
            return _scenes.TryGetValue(typeof(T), out scene);
        }
        
        /// <summary>
        /// Shows a scene associated with the specified bootstrap type.
        /// If the scene is already loaded, returns the existing bootstrap instance.
        /// </summary>
        /// <typeparam name="T">The bootstrap type that implements IBootstrap.</typeparam>
        /// <param name="setActive">Whether to set the loaded scene as the active scene.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the bootstrap instance.</returns>
        public async Awaitable<T> ShowSceneAsync<T>(bool setActive = false) where T : class, IBootstrap
        {
            if (_openScreens.TryGetValue(typeof(T), out var screen))
            {
                return (T)screen;
            }

            if (!TryGetScene<T>(out var scene))
            {
                Debug.LogError($"Scene '{typeof(T)}' is not loaded properly. did you generate the scene mapping?");
                return null;
            }

            var bootstrap = await scene.LoadAsync(setActive);

            if (bootstrap != null)
            {
                _openScreens[typeof(T)] = bootstrap;
            }

            return bootstrap as T;
        }

        /// <summary>
        /// Retrieves an existing bootstrap instance for the specified type.
        /// </summary>
        /// <typeparam name="T">The bootstrap type that implements IBootstrap.</typeparam>
        /// <returns>The bootstrap instance if found, otherwise null.</returns>
        public T GetSceneHandle<T>() where T : class, IBootstrap
        {
            return _openScreens.Values.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Unloads the specified scene and removes it from the active scenes registry.
        /// </summary>
        /// <param name="sceneHandle">The scene handle to unload.</param>
        /// <returns>A task that represents the asynchronous unload operation.</returns>
        public async Awaitable UnloadAsync(IAsyncScene sceneHandle)
        {
            if (sceneHandle == null)
            {
                Debug.LogError("Scene handle is null.");
                return;
            }

            if (!_sceneMapping.TryGetValue(sceneHandle.sceneType, out var type))
            {
                return;
            }
            
            _openScreens.Remove(type);

            await sceneHandle.SceneWillUnloadAsync();
            await sceneHandle.UnloadAsync();
        }
    }
}