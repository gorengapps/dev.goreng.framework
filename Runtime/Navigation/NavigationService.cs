using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frame.Runtime.Navigation
{
    public partial class NavigationService: MonoBehaviour
    {
        private readonly List<IAsyncScene> _scenes = new();
        private readonly Dictionary<string, IBootstrap> _openScreens = new();

        private const string SCENES_KEY = "scenes";
        
        private IEnumerator LoadAssetsCoroutine(string key, TaskCompletionSource<bool> completionSource)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key);
            
            while (!handle.IsDone)
            {
                yield return new WaitForEndOfFrame();
            }

            var locations = handle.Result;
            
            Addressables.LoadAssetsAsync<IAsyncScene>(locations, (data) => {
                _scenes.Add(data);
            });

            while (_scenes.Count != locations.Count)
            {
                yield return new WaitForEndOfFrame();
            }
            
            completionSource.SetResult(true);
        }
        
        private async Task Initialise()
        {
            if (_scenes.Count > 0)
            {
                return;
            }
            
            var source = new TaskCompletionSource<bool>();
            StartCoroutine(LoadAssetsCoroutine(SCENES_KEY, source));
            await source.Task;
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
            loadingScene.SceneWillUnload();
            
            await loadingScene.WhenDone(this);
            
            // Continue the runner
            await targetScene.Continue(this);
            
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
            if (_openScreens.ContainsKey(destination))
            {
                return (T)_openScreens[destination];
            }
            
            await Initialise();
            
            var supplementaryScene = FetchScene(destination);

            var instance = await supplementaryScene.Load(setActive);

            _openScreens[destination] = instance;
            
            return (T)instance;
        }

        public T GetSupplementarySceneHandle<T>(string type) where T: class
        {
            // Check for open scenes and remove it if its available
            if (_openScreens.ContainsKey(type))
            {
                return (T)_openScreens[type];
            }

            return null;
        }

        public async Task Unload(IAsyncScene sceneHandle)
        {
            // Check for open scenes and remove it if its available
            if (_openScreens.ContainsKey(sceneHandle.sceneType))
            {
                _openScreens.Remove(sceneHandle.sceneType);
            }
            
            sceneHandle.SceneWillUnload();
            await sceneHandle.Unload();
        }
    }
}