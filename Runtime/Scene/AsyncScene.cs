using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene
{
    public partial class AsyncScene : ScriptableObject
    {
        [SerializeField] private string _sceneType;
        [SerializeField] private AssetReference _sceneReference;
        
        private UnityEngine.SceneManagement.Scene _loadedScene;
        private IBootstrap _cachedBootstrap;

        public string sceneType => _sceneType;
        public UnityEngine.SceneManagement.Scene associatedScene => _loadedScene;

        /// <summary>
        /// Internally loads the scene, abstracts away hard logic
        /// </summary>
        /// <param name="setActiveScene">Flag that allows you to set the scene as a main scene</param>
        private async Task InternalLoad(bool setActiveScene)
        {
            if (_sceneReference.IsValid())
            {
                return;
            }
            
            _loadedScene = await IAsyncScene.loader.LoadScene(_sceneReference);
            
            if (setActiveScene)
            {
                SceneManager.SetActiveScene(_loadedScene);
            }
        }

        /// <summary>
        /// Unloads the level from the hierarchy
        /// </summary>
        private async Task UnloadScene()
        {
            if (!_sceneReference.IsValid())
            {
                return;
            }

            await _sceneReference.UnLoadScene().Task;
        }

        /// <summary>
        /// Runs the bootstrap in the loaded scene
        /// </summary>
        /// <param name="scene">The scene we want to start our bootstrap in</param>
        private async Task<IBootstrap> RunBootstrap(UnityEngine.SceneManagement.Scene scene)
        {
            var bootstrap = FindObjectsByType<AbstractBootstrap>(FindObjectsSortMode.None)
                .FirstOrDefault(x => x.gameObject.scene == scene);

            if (bootstrap == null)
            {
                return null;
            }
            
            bootstrap.Load(this);
            await bootstrap.OnBootstrapStartAsync();

            return bootstrap;
        }

        /// <summary>
        /// Explicitly unload the scene if this handle gets destroyed
        /// </summary>
        private async void OnDestroy()
        {
            await UnloadAsync();
        }
    }

    public partial class AsyncScene: IAsyncScene
    {
        public async Task SceneWillUnloadAsync()
        {
            if (_cachedBootstrap == null)
            {
                return;
            }
            
            await _cachedBootstrap.SceneWillUnloadAsync();
        }

        public async Task<IBootstrap> LoadAsync(bool setActive = true)
        {
            await InternalLoad(setActive);
            _cachedBootstrap = await RunBootstrap(_loadedScene);
            return _cachedBootstrap;
        }

        public async Task UnloadAsync()
        {
            if (_cachedBootstrap == null)
            {
                return;
            }
            
            await _cachedBootstrap.OnBootstrapStopAsync();
            await UnloadScene();
        }
    }
}