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
    public abstract class AbstractBootstrap : MonoBehaviour, IBootstrap
    {
        protected IAsyncScene _sceneContext;
        [InjectField] protected INavigationService _navigationService;
        
        private List<ICanvas> _canvasList = new List<ICanvas>();
        
        private List<ICanvas> FetchActiveCanvases()
        {
              var canvasList = new List<ICanvas>();
              
              foreach (var obj in _sceneContext.associatedScene.GetRootGameObjects())
              {
                  var canvases = obj.GetComponentsInChildren<ICanvas>();
  
                  foreach (var canvas in canvases)
                  {
                      if (canvas != null)
                      {
                          canvasList.Add(canvas);
                      }
                  }
              }
  
              return canvasList;
        }

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
                    
                field.SetValue(this, FetchCanvasViaType(field.FieldType));
            }

        }
        
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

        private void OnApplicationQuit()
        {
            OnBootstrapStop();
        }

        public virtual void OnBootstrapStop()
        {
            
        }

        public IAsyncScene GetSceneContext()
        {
            return _sceneContext;
        }

        public T FetchCanvas<T>() where T: ICanvas
        {
            return _canvasList.OfType<T>()
                .FirstOrDefault();
        }
        
        private ICanvas FetchCanvasViaType(Type type)
        {
            
            return _canvasList.FirstOrDefault(x => x.GetType().GetInterfaces().Contains(type));
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