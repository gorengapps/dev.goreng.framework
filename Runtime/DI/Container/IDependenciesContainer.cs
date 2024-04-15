using System;
using Frame.Runtime.DI.Collections;
using Frame.Runtime.DI.Provider;
using UnityEngine;

namespace Frame.Runtime.DI.Container
{
    public interface IDependenciesContainer
    {
        /// <summary>
        /// Construct the container using the registered dependencies
        /// </summary>
        /// <returns></returns>
        public IDependencyProvider Make();
        
        /// <summary>
        /// Register a dependency using a custom method
        /// </summary>
        /// <returns></returns>
        public void Register<T>(Func<IDependencyProvider, T> factory, bool singleton);
    }
}