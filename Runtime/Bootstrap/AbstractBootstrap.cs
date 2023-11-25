using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frame.Runtime.Canvas;
using Frame.Runtime.Scene;
using UnityEngine;

namespace Frame.Runtime.Bootstrap
{
    public abstract class AbstractBootstrap : MonoBehaviour, IBootstrap
    {
        protected IAsyncScene _sceneContext;
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
        
        public virtual async void OnBootstrapStart()
        {
            // We only need to resolve dependencies when the bootstrap is allowed to run
            var children = GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var child in children)
            {
                IBootstrap.provider?.Inject(child);
            }

            _canvasList = FetchActiveCanvases();
            
            await SceneWillLoad();
        }

        public virtual void OnBootstrapStop()
        {
            
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