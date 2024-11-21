using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Data;
using Frame.Runtime.Scene;
using UnityEngine;

namespace Frame.Runtime.Navigation
{
    public partial class NavigationService
    {
        private const string _scenesKey = "scenes";
        
        private List<IAsyncScene> _scenes = new List<IAsyncScene>();
        private readonly Dictionary<string, IBootstrap> _openScreens = new Dictionary<string, IBootstrap>();

        private readonly IDataService _dataService;
        
        public NavigationService(IDataService dataService)
        {
            _dataService = dataService;
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
        public async Task Navigate(string destination)
        {
            await Initialise();
            
            var targetScene = FetchScene(destination);

            if (targetScene == null)
            {
                Debug.LogError($"Scene {destination} is not loaded properly");
                return;
            }
            
            await targetScene.Load();
        }

        public async Task<T> ShowScene<T>(string destination, bool setActive = false)
        {
            await Initialise();
            
            if (_openScreens.TryGetValue(destination, out var screen))
            {
                return (T)screen;
            }
            
            var scene = FetchScene(destination);
            var bootstrap = await scene.Load(setActive);
            
            _openScreens[destination] = bootstrap;
            return (T)bootstrap;
        }
        
        public async Task ShowScene(string destination, bool setActive = false)
        {
            await Initialise();
            
            var scene = FetchScene(destination);
            var bootstrap = await scene.Load(setActive);
            
            _openScreens[destination] = bootstrap;
        }

        public T GetSceneHandle<T>(string type) where T: class
        {
            _openScreens.TryGetValue(type, out var screen);
            return (T)screen;
        }

        public async Task Unload(IAsyncScene sceneHandle)
        {
            // Check for open scenes and remove it if it's available
            if (_openScreens.ContainsKey(sceneHandle.sceneType))
            {
                _openScreens.Remove(sceneHandle.sceneType);
            }
            
            await sceneHandle.SceneWillUnload();
            await sceneHandle.Unload();
        }
    }
}