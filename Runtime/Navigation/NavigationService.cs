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
    [UsedImplicitly]
    public class NavigationService : INavigationService
    {
        private const string _scenesKey = "scenes";

        private readonly Dictionary<Type, IAsyncScene> _scenes = new Dictionary<Type, IAsyncScene>();
        private readonly Dictionary<Type, IBootstrap> _openScreens = new Dictionary<Type, IBootstrap>();
        private Dictionary<string, Type> _sceneMapping = new Dictionary<string, Type>();

        private readonly IDataService _dataService;
        private Awaitable _initialiseTask;
        private readonly object _initialiseLock = new object();

        public NavigationService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public Awaitable Initialise(Dictionary<string, Type> sceneMapping)
        {
            if (sceneMapping == null)
                throw new ArgumentNullException(nameof(sceneMapping));
            
            if (_initialiseTask != null)
            {
                return _initialiseTask;
            }

            lock (_initialiseLock)
            {
                if (_initialiseTask == null)
                {
                    _initialiseTask = InitialiseInternal(sceneMapping);
                }
            }

            return _initialiseTask;
        }

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

        private bool TryGetScene<T>(out IAsyncScene scene)
        {
            return _scenes.TryGetValue(typeof(T), out scene);
        }
        
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

        public T GetSceneHandle<T>() where T : class, IBootstrap
        {
            return _openScreens.Values.OfType<T>().FirstOrDefault();
        }

        public async Awaitable UnloadAsync(IAsyncScene sceneHandle)
        {
            if (sceneHandle == null)
            {
                Debug.LogError("Scene handle is null.");
                return;
            }

            if (!_sceneMapping.TryGetValue(sceneHandle.sceneType, out var type))
            {
                Debug.LogWarning($"Scene type '{sceneHandle.sceneType}' not found in scene mapping. Cannot unload scene.");
                return;
            }
            
            _openScreens.Remove(type);

            await sceneHandle.SceneWillUnloadAsync();
            await sceneHandle.UnloadAsync();
        }
    }
}