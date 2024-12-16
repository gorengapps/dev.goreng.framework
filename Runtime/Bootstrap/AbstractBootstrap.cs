using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Frame.Runtime.Canvas;
using Frame.Runtime.Navigation;
using Frame.Runtime.Scene;
using Framework.DI;
using UnityEngine;

namespace Frame.Runtime.Bootstrap
{
    public abstract partial class AbstractBootstrap : MonoBehaviour, IBootstrap
    {
        /// <summary>
        /// The navigation service that allows you to navigate easily from and to scenes.
        /// </summary>
        [InjectField]
        protected INavigationService _navigationService;

        /// <summary>
        /// Internal list that holds references to all the loaded ICanvas instances in the scene.
        /// </summary>
        private readonly List<ICanvas> _canvasList = new List<ICanvas>();

        /// <summary>
        /// The current scene context allowing you to unload the scene.
        /// </summary>
        protected IAsyncScene _sceneContext;

        /// <summary>
        /// Called when the bootstrap starts. Resolves dependencies, fetches canvases, and invokes scene load events.
        /// </summary>
        public virtual async Task OnBootstrapStartAsync()
        {
            // Resolve dependencies in child MonoBehaviours.
            var children = GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var child in children)
            {
                IBootstrap.provider?.Inject(child);
            }

            // Fetch and resolve canvases.
            FetchActiveCanvases();
            ResolveCanvases();

            // Invoke scene load events.
            await SceneWillLoadAsync();
        }

        /// <summary>
        /// Called when the bootstrap stops. Invokes scene unload events.
        /// </summary>
        public virtual async Task OnBootstrapStopAsync()
        {
            await SceneWillUnloadAsync();
        }

        /// <summary>
        /// Unloads the current scene using the navigation service.
        /// </summary>
        public virtual async Task UnloadAsync()
        {
            if (_navigationService != null && _sceneContext != null)
            {
                await _navigationService.UnloadAsync(_sceneContext);
            }
            else
            {
                Debug.LogWarning("Cannot unload scene: NavigationService or SceneContext is null.");
            }
        }

        /// <summary>
        /// Fetches a canvas of the specified type from the loaded canvases.
        /// </summary>
        /// <typeparam name="T">The type of canvas to fetch.</typeparam>
        /// <returns>The canvas instance if found; otherwise, null.</returns>
        public T FetchCanvas<T>() where T : ICanvas
        {
            return _canvasList.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Invoked before the scene unloads. Calls SceneWillUnload on all canvases.
        /// </summary>
        public virtual async Task SceneWillUnloadAsync()
        {
            var tasks = _canvasList.Select(canvas => canvas.SceneWillUnloadAsync());
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Invoked after the scene loads. Calls SceneWillLoad on all canvases.
        /// </summary>
        public virtual async Task SceneWillLoadAsync()
        {
            var tasks = _canvasList.Select(canvas => canvas.SceneWillLoadAsync());
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sets the scene context.
        /// </summary>
        /// <param name="sceneContext">The scene context to set.</param>
        public void Load(IAsyncScene sceneContext)
        {
            _sceneContext = sceneContext;
        }

        /// <summary>
        /// Fetches all active canvases in the scene and adds them to the canvas list.
        /// </summary>
        private void FetchActiveCanvases()
        {
            if (_sceneContext == null || !_sceneContext.associatedScene.IsValid())
            {
                Debug.LogError("Scene context is not valid.");
                return;
            }

            var canvases = _sceneContext.associatedScene.GetRootGameObjects()
                .SelectMany(obj => obj.GetComponentsInChildren<ICanvas>(true))
                .Distinct();

            _canvasList.Clear();
            _canvasList.AddRange(canvases);
        }

        /// <summary>
        /// Resolves canvases by setting fields marked with the FetchCanvas attribute.
        /// </summary>
        private void ResolveCanvases()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<FetchCanvasAttribute>(false) != null)
                {
                    var canvas = FetchCanvasByType(field.FieldType);
                    if (canvas != null)
                    {
                        field.SetValue(this, canvas);
                    }
                    else
                    {
                        Debug.LogWarning($"Canvas of type '{field.FieldType}' not found for field '{field.Name}' in '{GetType().Name}'.");
                    }
                }
            }
        }

        /// <summary>
        /// Fetches a canvas from the canvas list that matches the specified type.
        /// </summary>
        /// <param name="type">The type of canvas to fetch.</param>
        /// <returns>The canvas instance if found; otherwise, null.</returns>
        private ICanvas FetchCanvasByType(Type type)
        {
            return _canvasList.FirstOrDefault(type.IsInstanceOfType);
        }

        private async void OnApplicationQuit()
        {
            try
            {
                await OnBootstrapStopAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during OnApplicationQuit: {ex.Message}");
            }
        }
    }
}