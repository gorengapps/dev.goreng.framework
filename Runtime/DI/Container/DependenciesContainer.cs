using System;
using System.Collections.Generic;
using System.Linq;
using Frame.Runtime.DI.Collections;
using Frame.Runtime.DI.Provider;
using UnityEngine;

namespace Frame.Runtime.DI.Container
{
    [CreateAssetMenu(fileName = "Container", menuName = "Framework/Dependencies/Create Container")]
    public partial class DependenciesContainer: ScriptableObject
    {
        /// <summary>
        /// This defines all the singletons that should stay alive per scene, or cross scene if they are single
        /// </summary>
        [Header("This defines all the singletons that should stay alive per scene, or cross scene if they are single")]
        [SerializeField] private List<MonoBehaviour> _singletons;
        
        /// <summary>
        /// This defines all the factories that produce unique elements, make sure there is one interface
        /// </summary>
        [Header("This defines all the factories that produce unique elements, make sure there is one interface")]
        [SerializeField] private List<MonoBehaviour> _factories;
        
        /// <summary>
        /// This defines all the entities that should be unique per request
        /// </summary>
        [Header("This defines all the entities that should be unique per request, registered under their concrete type")]
        [SerializeField] private List<MonoBehaviour> _entities;
    }

    public partial class DependenciesContainer : IDependenciesContainer
    {
        public IDependencyProvider Make()
        {
            var collection = new DependenciesCollection();
            
            foreach (var dependency in _singletons)
            {
                collection.Add(
                    new Dependency
                    {
                        factory = DependencyFactory.FromPrefab(dependency),
                        types = dependency.GetType().GetInterfaces().ToList(),
                        isSingleton = true
                    }
                );    
            }
            
            foreach (var dependency in _factories)
            {
                collection.Add(
                    new Dependency
                    {
                        factory = DependencyFactory.FromPrefab(dependency),
                        types = dependency.GetType().GetInterfaces().ToList(),
                        isSingleton = false
                    }
                );    
            }

            foreach (var dependency in _entities)
            {
                collection.Add(
                    new Dependency
                    {
                        factory = DependencyFactory.FromPrefab(dependency),
                        types = new List<Type> { dependency.GetType() },
                        isSingleton = false
                    }
                ); 
            }

            return new BaseDependencyProvider(collection);
        }
    }
}