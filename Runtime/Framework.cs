using Frame.Runtime.Data;
using Frame.Runtime.DI.Container;
using Frame.Runtime.Navigation;
using Frame.Runtime.RunLoop;

namespace Frame.Runtime
{
    public static class Framework
    {
        public static void RegisterBaseDependencies(IDependenciesContainer container)
        {
            container.Register<IDataService>(
                provider => new DataService(provider.Get<IRunLoop>()),
                true
            );   
        
            container.Register<INavigationService>(
                provider => new NavigationService(provider.Get<IDataService>(), provider.Get<IRunLoop>()),
                true
            );
        }
    }
}