using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Frame.Runtime.Bootstrap;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Frame.Runtime.Scene
{
    [CreateAssetMenu(fileName = "AsyncScene", menuName = "Framework/Scene/Create Scene")]
    public partial class AsyncScene : ScriptableObject
    {
        [SerializeField] private string _sceneType;
        [SerializeField] private AssetReference _sceneReference;

        private SceneInstance _sceneInstance;
        private UnityEngine.SceneManagement.Scene _loadedScene;
        private IBootstrap _cachedBootstrap;

        public string sceneType => _sceneType;
        public UnityEngine.SceneManagement.Scene associatedScene => _loadedScene;

        /// <summary>
        /// Internally loads the scene, abstracts away hard logic
        /// </summary>
        /// <param name="activateOnLoad">Flag that allow a scene to be activated when its loaded in memory</param>
        /// <param name="setActiveScene">Flag that allows you to set the scene as a main scene</param>
        private async Task InternalLoad(bool activateOnLoad, bool setActiveScene)
        {
            Action<AsyncOperationHandle<SceneInstance>> onComplete = null;

            onComplete = (result) =>
            {
                _loadedScene = result.Result.Scene;

                if (activateOnLoad)
                {
                    if (setActiveScene)
                    {
                        SceneManager.SetActiveScene(result.Result.Scene);
                    }

                    RunBootstrap(result.Result.Scene);
                }

                if (onComplete != null)
                {
                    result.Completed -= onComplete;
                }
            };

            if (!_sceneReference.IsValid())
            {
                var handle = _sceneReference.LoadSceneAsync(LoadSceneMode.Additive, activateOnLoad);
                handle.Completed += onComplete;

                await handle.Task;

                _sceneInstance = handle.Result;
            }

            _sceneInstance = (SceneInstance)_sceneReference.OperationHandle.Result;
        }

        /// <summary>
        /// Coroutine that asynchronously sets an scene to active, we need to use a coroutine instead of a task because
        /// WebGL only can manage one thread. :(
        /// </summary>
        /// <param name="completion">The action that will run once activation is complete</param>
        /// <param name="setActiveScene">Sets the scene as the current active scene</param>
        private IEnumerator ActivateSceneCoroutine(Action completion, bool setActiveScene)
        {
            var instance = _sceneInstance.ActivateAsync();

            while (!instance.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            // Another undocumented Unity feature, this will not be done after execution, but a few cpu cycles later
            if (setActiveScene)
            {
                SceneManager.SetActiveScene(_loadedScene);

                // Wait until the active scene is changed
                while (SceneManager.GetActiveScene().name != _loadedScene.name)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            completion?.Invoke();
        }

        /// <summary>
        /// Method will convert the Scene Activation coroutine into an awaitable async version that works with WebGL
        /// </summary>
        /// <param name="runner">The MonoBehaviour that will run the routine</param>
        private async Task ActivateScene(MonoBehaviour runner)
        {
            // We didnt have the scene loaded yet
            if (_sceneInstance.Equals(default))
            {
                return;
            }

            // Create a task that can signal when loading is done
            var source = new TaskCompletionSource<bool>();

            Action completion = () => { source.SetResult(true); };

            runner.StartCoroutine(ActivateSceneCoroutine(completion, true));

            await source.Task;

            RunBootstrap(_loadedScene);
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
        private void RunBootstrap(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                _cachedBootstrap = root.GetComponent<IBootstrap>();

                _cachedBootstrap?.Load(this);
                _cachedBootstrap?.OnBootstrapStart();

                if (_cachedBootstrap != null)
                {
                    break;
                }
            }
        }
    }

    public partial class AsyncScene: IAsyncScene
    {
        public async Task Preload()
        {
            await InternalLoad(false, true);
        }

        public async Task SceneWillUnload()
        {
            if (_cachedBootstrap == null)
            {
                return;
            }
            
            await _cachedBootstrap.SceneWillUnload();
        }

        public virtual Task WhenDone(MonoBehaviour runner)
        {
            return Task.CompletedTask;
        }

        public async Task Continue(MonoBehaviour runner)
        {
            await ActivateScene(runner);
        }

        public async Task<IBootstrap> Load(bool setActive = true)
        {
            await InternalLoad(true, setActive);
            return _cachedBootstrap;
        }

        public async Task Unload()
        {
            _cachedBootstrap?.OnBootstrapStop();
            await UnloadScene();
        }
    }
}