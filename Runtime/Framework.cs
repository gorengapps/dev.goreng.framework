using Frame.Runtime.Bootstrap;
using Frame.Runtime.Canvas;
using Frame.Runtime.Data;
using Frame.Runtime.Navigation;
using Frame.Runtime.Tasks;
using Framework.DI.Container;
using Framework.DI.Provider;
using Framework.Loop;
using UnityEngine;

namespace Frame.Runtime
{
    public static class Framework
    {
        public static void RegisterBaseDependencies(IDependenciesContainer container)
        {
            container.Register<IDataService, DataService>();
            container.Register<INavigationService, NavigationService>();
            container.Register<IRunLoop>((_) => new GameObject("Runloop").AddComponent<BaseRunLoop>());
        }

        public static void Initialize(IDependencyProvider provider)
        {
            IBootstrap.SetProvider(provider);
            IView.SetProvider(provider);
            CoroutineTask.SetRunLoop(provider.Get<IRunLoop>());
        }
    }
}