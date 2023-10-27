﻿using System;
using System.Runtime.Serialization;
using Frame.Runtime.DI.Provider;
using UnityEngine;

namespace Frame.Runtime.DI
{
    public static class DependencyFactory
    {
        public delegate object Delegate(IDependencyProvider provider);

        /// <summary>
        /// Resolves a dependency from a base C# class
        /// </summary>
        /// <typeparam name="T">The class requested</typeparam>
        /// <returns></returns>
        public static Delegate FromClass<T>() where T : class, new()
        {
            return (provider) =>
            {
                var type = typeof(T);
                var obj = FormatterServices.GetUninitializedObject(type);

                provider.Inject(obj);

                type.GetConstructor(Type.EmptyTypes)?.Invoke(obj, null);

                return (T)obj;
            };
        }

        /// <summary>
        /// Resolves a dependency from a base Unity Prefab
        /// </summary>
        /// <param name="prefab">The prefab we want to use</param>
        /// <typeparam name="T">The type we want to resolve</typeparam>
        /// <returns></returns>
        public static Delegate FromPrefab<T>(T prefab) where T : MonoBehaviour
        {
            return (provider) =>
            {
                var wasActive = prefab.gameObject.activeSelf;
                prefab.gameObject.SetActive(false);
                
                var instance = GameObject.Instantiate(prefab);
                instance.name =  $"injected_{prefab.name}";
                
                prefab.gameObject.SetActive(wasActive);
                
                var children = instance.GetComponentsInChildren<MonoBehaviour>(true);
                
                foreach (var child in children)
                {
                    provider.Inject(child);
                }
                
                instance.gameObject.SetActive(wasActive);
                
                return instance.GetComponent<T>();
            };
        }

        /// <summary>
        /// Resolves a dependency from a GameObject
        /// </summary>
        /// <param name="instance">The instance we want to resolve from</param>
        /// <typeparam name="T">The type we want resolved</typeparam>
        /// <returns></returns>
        public static Delegate FromGameObject<T>(T instance) where T : MonoBehaviour
        {
            return (provider) =>
            {
                var children = instance.
                    GetComponentsInChildren<MonoBehaviour>(true);
                
                foreach (var child in children)
                {
                    provider.Inject(child);
                }
                
                return instance;
            };
        }
    }
}