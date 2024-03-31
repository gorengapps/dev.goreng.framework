using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Data;
using Frame.Runtime.RunLoop;
using Frame.Runtime.Scene;
using UnityEngine;

namespace Frame.Runtime.Navigation
{
    public partial class NavigationService
    {
        private const string _scenesKey = "scenes";
        
        private List<IAsyncScene> _scenes = new();
        private readonly Dictionary<string, IBootstrap> _openScreens = new();

        private readonly IDataService _dataService;
        private readonly IRunLoop _runLoop;
        

        public NavigationService(IDataService dataService, IRunLoop runLoop)
        {
            _dataService = dataService;
            _runLoop = runLoop;
        }
        
        private async Task Initialise()
        {
            if (_scenes.Count > 0)
            {
                return;
            }
            
            _scenes = await _dataService.LoadList<IAsyncScene>(_scenesKey);
        }
        
        private IAsyncScene FetchScene(string type)
        {
            return _scenes
                .FirstOrDefault(x => x.sceneType == type);
        }
    }

    public partial class NavigationService : INavigationService
    {
        public async Task NavigateTo(string destination, string intermediate)
        {
            await Initialise();
            
            var loadingScene = FetchScene(intermediate);
            var targetScene = FetchScene(destination);

            if (loadingScene == null || targetScene == null)
            {
                Debug.LogError("[Navigation manager] scenes are not loaded properly");
                return;
            }
            
            // Load our loading scene and preload our target scene
            await loadingScene.Load();
            await targetScene.Preload();

            // Wait for our loading scene to be done with animating
            await loadingScene.SceneWillUnload();
            
            await loadingScene.WhenDone(_runLoop);
            
            // Continue the runner
            await targetScene.Continue(_runLoop);
            
            // Unload our loading scene
            await loadingScene.Unload();
        }

        public async Task Navigate(string destination)
        {
            await Initialise();
            
            var targetScene = FetchScene(destination);

            if (targetScene == null)
            {
                Debug.LogError("[Navigation manager] scenes are not loaded properly");
                return;
            }
            
            await targetScene.Load();
        }

        public async Task<T> ShowSupplementaryScene<T>(string destination, bool setActive = false)
        {
            await Initialise();
            
            if (_openScreens.TryGetValue(destination, out var screen))
            {
                return (T)screen;
            }
            
            var supplementaryScene = FetchScene(destination);

            var instance = await supplementaryScene.Load(setActive);

            _openScreens[destination] = instance;
            
            return (T)instance;
        }

        public T GetSupplementarySceneHandle<T>(string type) where T: class
        {
            _openScreens.TryGetValue(type, out var screen);
            return (T)screen;
        }

        public async Task Unload(IAsyncScene sceneHandle)
        {
            // Check for open scenes and remove it if its available
            if (_openScreens.ContainsKey(sceneHandle.sceneType))
            {
                _openScreens.Remove(sceneHandle.sceneType);
            }
            
            await sceneHandle.SceneWillUnload();
            await sceneHandle.Unload();
        }
    }
}