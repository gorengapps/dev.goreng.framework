using System;
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
    /// <summary>
    /// Core framework class that provides initialization and dependency registration functionality.
    /// This class serves as the main entry point for setting up the Frame.Runtime framework.
    /// </summary>
    public static class Framework
    {
        /// <summary>
        /// Registers the base dependencies required by the Frame.Runtime framework.
        /// This includes core services like data management, navigation, and run loop.
        /// </summary>
        /// <param name="container">The dependency container to register services with.</param>
        public static void RegisterBaseDependencies(IDependenciesContainer container)
        {
            container.Register<IDataService, DataService>();
            container.Register<INavigationService, NavigationService>();
            container.Register<IRunLoop>((_) => new GameObject("Runloop").AddComponent<BaseRunLoop>());
        }

        /// <summary>
        /// Initializes the framework with the provided dependency provider.
        /// This method configures all framework components to use the specified provider
        /// for dependency resolution and sets up the run loop for coroutine execution.
        /// </summary>
        /// <param name="provider">The dependency provider to use for service resolution.</param>
        public static void Initialize(IDependencyProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
                
            IBootstrap.SetProvider(provider);
            IView.SetProvider(provider);
            CoroutineTask.SetRunLoop(provider.Get<IRunLoop>());
        }
    }
}