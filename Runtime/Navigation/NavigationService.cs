using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Data;
using Frame.Runtime.Scene;
using UnityEngine;

namespace Frame.Runtime.Navigation
{
    public class NavigationService : INavigationService
    {
        private const string _scenesKey = "scenes";

        private readonly Dictionary<string, IAsyncScene> _scenes = new Dictionary<string, IAsyncScene>();
        private readonly Dictionary<string, IBootstrap> _openScreens = new Dictionary<string, IBootstrap>();

        private readonly IDataService _dataService;
        private Task _initialiseTask;
        private readonly object _initialiseLock = new object();

        public NavigationService(IDataService dataService)
        {
            _dataService = dataService;
        }

        private Task Initialise()
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

            foreach (var scene in scenesList)
            {
                if (scene != null && !string.IsNullOrEmpty(scene.sceneType))
                {
                    _scenes[scene.sceneType] = scene;
                }
            }
        }

        private bool TryGetScene(string type, out IAsyncScene scene)
        {
            return _scenes.TryGetValue(type, out scene);
        }

        public async Task Navigate(string destination)
        {
            await Initialise();

            if (!TryGetScene(destination, out var targetScene))
            {
                Debug.LogError($"Scene '{destination}' is not loaded properly.");
                return;
            }

            await targetScene.LoadAsync();
        }

        public async Task<T> ShowScene<T>(string destination, bool setActive = false) where T : class, IBootstrap
        {
            await Initialise();

            if (_openScreens.TryGetValue(destination, out var screen))
            {
                return screen as T;
            }

            if (!TryGetScene(destination, out var scene))
            {
                Debug.LogError($"Scene '{destination}' is not loaded properly.");
                return null;
            }

            var bootstrap = await scene.LoadAsync(setActive);

            if (bootstrap != null)
            {
                _openScreens[destination] = bootstrap;
            }

            return bootstrap as T;
        }

        public async Task ShowScene(string destination, bool setActive = false)
        {
            await Initialise();

            if (_openScreens.ContainsKey(destination))
            {
                // Scene is already open
                return;
            }

            if (!TryGetScene(destination, out var scene))
            {
                Debug.LogError($"Scene '{destination}' is not loaded properly.");
                return;
            }

            var bootstrap = await scene.LoadAsync(setActive);

            if (bootstrap != null)
            {
                _openScreens[destination] = bootstrap;
            }
        }

        public T GetSceneHandle<T>() where T : class, IBootstrap
        {
            return _openScreens.Values.OfType<T>().FirstOrDefault();
        }

        public async Task Unload(IAsyncScene sceneHandle)
        {
            if (sceneHandle == null)
            {
                Debug.LogError("Scene handle is null.");
                return;
            }

            _openScreens.Remove(sceneHandle.sceneType);

            await sceneHandle.SceneWillUnloadAsync();
            await sceneHandle.UnloadAsync();
        }
    }
}