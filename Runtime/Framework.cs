using Frame.Runtime.Data;
using Frame.Runtime.Navigation;
using Framework.DI.Container;
using Framework.Loop;

namespace Frame.Runtime
{
    public static class Framework
    {
        public static void RegisterBaseDependencies(IDependenciesContainer container)
        {
            container.Register<IDataService>(
                provider => new DataService(),
                true
            );   
        
            container.Register<INavigationService>(
                provider => new NavigationService(provider.Get<IDataService>()),
                true
            );
        }
    }
}