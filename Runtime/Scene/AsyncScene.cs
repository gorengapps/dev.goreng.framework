using System.Linq;
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
        private async Awaitable InternalLoad(bool setActiveScene)
        {
            if (!_sceneReference.IsValid())
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
        private async Awaitable UnloadScene()
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
        private async Awaitable<IBootstrap> RunBootstrap(UnityEngine.SceneManagement.Scene scene)
        {
            var bootstrap = FindObjectsByType<AbstractBootstrap>(FindObjectsSortMode.None)
                .FirstOrDefault(x => x.gameObject.scene == scene);

            if (!bootstrap)
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
        public async Awaitable SceneWillUnloadAsync()
        {
            if (_cachedBootstrap == null)
            {
                Debug.LogError("Attempting to unload scene without a bootstrap instance. " +
                               "this can happen if you are you awaiting a long running task inside " +
                               "the OnBootstrapStartAsync");
                return;
            }
            
            await _cachedBootstrap.SceneWillUnloadAsync();
        }

        public async Awaitable<IBootstrap> LoadAsync(bool setActive = true)
        {
            await InternalLoad(setActive);
            _cachedBootstrap = await RunBootstrap(_loadedScene);
            return _cachedBootstrap;
        }

        public async Awaitable UnloadAsync()
        {
            if (_cachedBootstrap == null)
            {
                Debug.LogError("Attempting to unload scene without a bootstrap instance. " +
                               "this can happen if you are you awaiting a long running task inside " +
                               "the OnBootstrapStartAsync");
                return;
            }
            
            await _cachedBootstrap.OnBootstrapStopAsync();
            await UnloadScene();
        }
    }
}