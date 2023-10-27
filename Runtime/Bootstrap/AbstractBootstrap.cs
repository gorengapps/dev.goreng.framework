using System.Collections.Generic;
using System.Linq;
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
                var canvas = obj.GetComponent<ICanvas>();

                if (canvas != null)
                {
                    canvasList.Add(canvas);
                }
            }

            return canvasList;
        }
        
        public virtual void OnBootstrapStart()
        {
            // We only need to resolve dependencies when the bootstrap is allowed to run
            var children = GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var child in children)
            {
                IBootstrap.provider?.Inject(child);
            }

            _canvasList = FetchActiveCanvases();
            
            SceneWillLoad();
        }

        public virtual void OnBootstrapStop()
        {
            
        }

        public T FetchCanvas<T>() where T: ICanvas
        {
            return _canvasList.OfType<T>()
                .FirstOrDefault();
        }

        public virtual void SceneWillUnload()
        {
            _canvasList.ForEach(canvas => canvas.SceneWillUnload());
        }

        public virtual void SceneWillLoad()
        {
            _canvasList.ForEach(canvas => canvas.SceneWillLoad());
        }

        public void Load(IAsyncScene sceneContext)
        {
            _sceneContext = sceneContext;
        }
    }
}