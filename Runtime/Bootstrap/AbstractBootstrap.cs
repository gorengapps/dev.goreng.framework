using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Frame.Runtime.Canvas;
using Frame.Runtime.Extensions;
using Frame.Runtime.Navigation;
using Frame.Runtime.Scene;
using Framework.DI;
using Framework.Events;
using UnityEngine;

namespace Frame.Runtime.Bootstrap
{
    public abstract partial class AbstractBootstrap : MonoBehaviour, IBootstrap
    {
        /// <summary>
        /// The navigation service that allows you to navigate easily from and to scenes.
        /// </summary>
        [InjectField] protected INavigationService _navigationService;

        /// <summary>
        /// Can hold relevant subscriptions
        /// </summary>
        protected readonly DisposeBag _disposeBag = new DisposeBag();
        
        /// <summary>
        /// Internal list that holds references to all the loaded IView instances in the scene.
        /// </summary>
        private readonly List<IView> _viewList = new List<IView>();

        /// <summary>
        /// The current scene context allowing you to unload the scene.
        /// </summary>
        protected IAsyncScene _sceneContext;
        
        /// <summary>
        /// A bool which signals if a bootstrap has been started yet.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Called when the bootstrap starts. Resolves dependencies, fetches canvases, and invokes scene load events.
        /// </summary>
        public virtual async Awaitable OnBootstrapStartAsync()
        {
            // Resolve dependencies in child MonoBehaviours.
            var children = GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var child in children)
            {
                IBootstrap.provider?.Inject(child);
            }

            // Fetch and resolve canvases.
            FetchActiveViews();
            ResolveViews();
            
            foreach (var view in _viewList)
            {
                IBootstrap.provider?.Inject(view);
            }

            // Invoke scene load events.
            await SceneWillLoadAsync();
            
            _initialized = true;
        }

        /// <summary>
        /// Called when the bootstrap stops. Invokes scene unload events.
        /// </summary>
        public virtual async Awaitable OnBootstrapStopAsync()
        {
            _disposeBag.Dispose(); 
            await SceneWillUnloadAsync();
        }

        public virtual Awaitable OnBootstrapUpdateAsync()
        {
            return Awaitable.EndOfFrameAsync();
        }

        public virtual Awaitable OnBootstrapLateUpdateAsync()
        {
            return Awaitable.EndOfFrameAsync();
        }

        /// <summary>
        /// Unloads the current scene using the navigation service.
        /// </summary>
        public virtual async Awaitable UnloadAsync()
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
        /// Fetches a view of the specified type from the loaded views.
        /// </summary>
        /// <typeparam name="T">The type of view to fetch.</typeparam>
        /// <returns>The canvas instance if found; otherwise, null.</returns>
        public T FetchView<T>() where T : IView
        {
            return _viewList.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Invoked before the scene unloads. Calls SceneWillUnload on all canvases.
        /// </summary>
        public virtual async Awaitable SceneWillUnloadAsync()
        {
            var tasks = _viewList
                .Select(canvas => canvas.ViewWillUnloadAsync().AsTask());
            
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Invoked after the scene loads. Calls SceneWillLoad on all canvases.
        /// </summary>
        public virtual async Awaitable SceneWillLoadAsync()
        {
            var tasks = _viewList
                .Select(canvas => canvas.ViewWillLoadAsync().AsTask());
            
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
        private void FetchActiveViews()
        {
            if (_sceneContext == null || !_sceneContext.associatedScene.IsValid())
            {
                Debug.LogError("Scene context is not valid.");
                return;
            }

            var views = _sceneContext.associatedScene.GetRootGameObjects()
                .SelectMany(obj => obj.GetComponentsInChildren<IView>(true))
                .Distinct();

            _viewList.Clear();
            _viewList.AddRange(views);
        }

        private static readonly Dictionary<Type, List<FieldInfo>> _cachedViewFields = new Dictionary<Type, List<FieldInfo>>();

        private List<FieldInfo> GetCachedViewFields()
        {
            var type = GetType();
            if (_cachedViewFields.TryGetValue(type, out var fields))
            {
                return fields;
            }

            fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetCustomAttribute<FetchViewAttribute>(false) != null)
                .ToList();

            _cachedViewFields[type] = fields;
            return fields;
        }

        /// <summary>
        /// Resolves canvases by setting fields marked with the FetchCanvas attribute, using cached fields for performance.
        /// </summary>
        private void ResolveViews()
        {
            var fields = GetCachedViewFields();

            foreach (var field in fields)
            {
                var view = FetchCanvasByType(field.FieldType);
               
                if (view != null)
                {
                    field.SetValue(this, view);
                }
                else
                {
                    Debug.LogWarning($"View of type '{field.FieldType}' not found for field '{field.Name}' in '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Fetches a canvas from the canvas list that matches the specified type.
        /// </summary>
        /// <param name="type">The type of canvas to fetch.</param>
        /// <returns>The canvas instance if found; otherwise, null.</returns>
        private IView FetchCanvasByType(Type type)
        {
            return _viewList.FirstOrDefault(type.IsInstanceOfType);
        }
        
        private async void Update()
        {
            if (!_initialized)
            {
                return;
            }

            await OnBootstrapUpdateAsync();
        }

        private async void LateUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            await OnBootstrapLateUpdateAsync();
        }

        private async void OnApplicationQuit()
        {
            try
            {
                await OnBootstrapStopAsync();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}