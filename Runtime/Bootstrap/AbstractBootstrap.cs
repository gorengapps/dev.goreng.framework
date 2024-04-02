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
    public abstract partial class AbstractBootstrap : MonoBehaviour
    {
        /// <summary>
        /// The navigation service that allows you to navigate easily from and to scenes
        /// </summary>
        [InjectField] protected INavigationService _navigationService;
        
        /// <summary>
        /// Internal list that holds references to all the loaded ICanvas in the scene
        /// </summary>
        private List<ICanvas> _canvasList = new List<ICanvas>();
        
        /// <summary>
        /// The current scene context allowing you to unload the scene
        /// </summary>
        protected IAsyncScene _sceneContext;
        
        /// <summary>
        /// Traverse all the root objects and find all ICanvas instances
        /// </summary>
        /// <returns></returns>
        private List<ICanvas> FetchActiveCanvases()
        {
              var canvasList = new List<ICanvas>();
              
              foreach (var obj in _sceneContext.associatedScene.GetRootGameObjects())
              {
                  var canvases = obj.GetComponentsInChildren<ICanvas>();

                  canvasList.AddRange(canvases.Where(canvas => canvas != null));
              }
  
              return canvasList;
        }

        /// <summary>
        /// Parsing the custom [FetchCanvas] attribute to prefetch the canvas for you
        /// </summary>
        private void ResolveCanvases()
        {
            var fields = GetType().GetFields(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.DeclaredOnly | 
                BindingFlags.Instance
            );
                
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<FetchCanvasAttribute>(false) == null)
                {
                    continue;
                }
                    
                field.SetValue(this, FetchCanvasViaInterface(field.FieldType));
            }

        }
        
        private ICanvas FetchCanvasViaInterface(Type type)
        {
            return _canvasList.FirstOrDefault(canvas => canvas
                .GetType()
                .GetInterfaces()
                .Contains(type));
        }
        
        private void OnApplicationQuit()
        {
            OnBootstrapStop();
        }
    }

    public abstract partial class AbstractBootstrap: IBootstrap
    {
        public virtual async void OnBootstrapStart()
        {
            // We only need to resolve dependencies when the bootstrap is allowed to run
            var children = GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var child in children)
            {
                IBootstrap.provider?.Inject(child);
            }

            _canvasList = FetchActiveCanvases();
            
            ResolveCanvases();
            
            await SceneWillLoad();
        }
        
        public virtual void OnBootstrapStop()
        {
            
        }

        public virtual async Task Unload()
        {
            await _navigationService.Unload(_sceneContext);
        }

        public T FetchCanvas<T>() where T: ICanvas
        {
            return _canvasList.OfType<T>()
                .FirstOrDefault();
        }
        
        public virtual async Task SceneWillUnload()
        {
            var tasks = _canvasList
                .Select(canvas => canvas.SceneWillUnload());

            await Task.WhenAll(tasks);
        }

        public virtual async Task SceneWillLoad()
        {
            var tasks = _canvasList
                .Select(canvas => canvas.SceneWillLoad());
            
            await Task.WhenAll(tasks);
        }

        public void Load(IAsyncScene sceneContext)
        {
            _sceneContext = sceneContext;
        }
    } 
}