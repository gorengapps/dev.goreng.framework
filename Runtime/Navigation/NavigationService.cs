using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Frame.Runtime.Attributes;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Data;
using Frame.Runtime.Scene;
using UnityEngine;

namespace Frame.Runtime.Navigation
{
    public class NavigationService : INavigationService
    {
        private const string _scenesKey = "scenes";

        private readonly Dictionary<Type, IAsyncScene> _scenes = new Dictionary<Type, IAsyncScene>();
        private readonly Dictionary<Type, IBootstrap> _openScreens = new Dictionary<Type, IBootstrap>();
        private Dictionary<string, Type> _sceneMapping = new Dictionary<string, Type>();

        private readonly IDataService _dataService;
        private Task _initialiseTask;
        private readonly object _initialiseLock = new object();

        public NavigationService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public Task Initialise()
        {
            if (_initialiseTask != null)
            {
                return _initialiseTask;
            }

            lock (_initialiseLock)
            {
                _initialiseTask = InitialiseInternal();
            }

            return _initialiseTask;
        }

        private async Task InitialiseInternal()
        {
            var scenesList = await _dataService.LoadList<IAsyncScene>(_scenesKey);
            
            if (scenesList == null)
            {
                Debug.LogError("Failed to load scenes from data service.");
                return;
            }
            
            _scenes.Clear();

            var validAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.Contains("Unity") && !x.FullName.Contains("System"));
            
            _sceneMapping = validAssemblies
                .SelectMany(x => x.GetTypes()) // Get all types
                .Where(t => typeof(IBootstrap).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract) // Find all IBootstraps
                .Select(t => new { Type = t, Attribute = t.GetCustomAttribute<SceneAttribute>() }) // Get all IBootstrap where we have a scene attribute
                .Where(x => x.Attribute != null) // Filter out empty ones
                .ToDictionary(x => x.Attribute.sceneName, x => x.Type.GetInterfaces().FirstOrDefault(t => t != typeof(IBootstrap)) ?? x.Type);
            
            foreach (var scene in scenesList.Where(scene => scene != null && !string.IsNullOrEmpty(scene.sceneType)))
            {
                if (_sceneMapping.TryGetValue(scene.sceneType, out var bootstrap))
                {
                    _scenes[bootstrap] = scene;
                }
            }
        }

        private bool TryGetScene<T>(out IAsyncScene scene)
        {
            return _scenes.TryGetValue(typeof(T), out scene);
        }
        
        public async Task<T> ShowSceneAsync<T>(bool setActive = false) where T : class, IBootstrap
        {
            await Initialise();

            if (_openScreens.TryGetValue(typeof(T), out var screen))
            {
                return (T)screen;
            }

            if (!TryGetScene<T>(out var scene))
            {
                Debug.LogError($"Scene '{typeof(T)}' is not loaded properly.");
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

        public async Task UnloadAsync(IAsyncScene sceneHandle)
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
            
            if (_openScreens.ContainsKey(type))
            {
                _openScreens.Remove(type);
            }

            await sceneHandle.SceneWillUnloadAsync();
            await sceneHandle.UnloadAsync();
        }
    }
}