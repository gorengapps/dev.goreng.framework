using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frame.Runtime
{
    /// <summary>
    /// Unity doesnt allow serialisation of interfaces through the editor, this sealed class will act as intermediary
    /// So that we can still set interfaces through the editor
    /// </summary>
    /// <typeparam name="T">The interface we want to be serialized</typeparam>
    [Serializable]
    public sealed class IRef<T>: ISerializationCallbackReceiver where T: class
    {
        public Object target;
        
        public T value => target as T;
        public static implicit operator bool(IRef<T> iRef) => iRef.target != null;
        
        void OnValidate()
        {
            if (target is T)
            {
                return;
            }
            
            if (target is not GameObject gameObject)
            {
                target = null;
                return;
            }

            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (component is not T)
                {
                    continue;
                }
                
                target = component;
                break;
            }
        }
 
        void ISerializationCallbackReceiver.OnBeforeSerialize() => OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}