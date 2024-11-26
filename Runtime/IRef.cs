using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frame.Runtime
{
    /// <summary>
    /// A serializable reference that allows interfaces to be assigned through the Unity Inspector.
    /// Since Unity cannot serialize interfaces directly, this class acts as an intermediary,
    /// enabling you to assign objects that implement the interface <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The interface type that you want to reference.</typeparam>
    [Serializable]
    public sealed class IRef<T> : ISerializationCallbackReceiver where T : class
    {
        [SerializeField]
        private Object _target;

        /// <summary>
        /// Gets the value of the interface implemented by the target object.
        /// </summary>
        public T value { get; private set; }

        /// <summary>
        /// Implicit conversion to bool to allow for null checks.
        /// </summary>
        /// <param name="iRef">The IRef instance.</param>
        public static implicit operator bool(IRef<T> iRef) => iRef != null && iRef.value != null;

        /// <summary>
        /// Implicit conversion to the interface type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="iRef">The IRef instance.</param>
        public static implicit operator T(IRef<T> iRef) => iRef?.value;

        /// <summary>
        /// Called before the object is serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            Validate();
        }

        /// <summary>
        /// Called after the object has been deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            Validate();
        }

        /// <summary>
        /// Validates and updates the Value property based on the current target.
        /// </summary>
        private void Validate()
        {
            if (_target == null)
            {
                value = null;
                return;
            }

            // If the target directly implements T
            if (_target is T validTarget)
            {
                value = validTarget;
                return;
            }

            // If the target is a GameObject, search its components
            if (_target is GameObject gameObject)
            {
                value = gameObject.GetComponent<T>();
                if (value == null)
                {
                    Debug.LogWarning($"GameObject '{gameObject.name}' does not have a component that implements '{typeof(T)}'.", gameObject);
                }
                return;
            }

            // If the target is a Component, get the GameObject and search its components
            if (_target is Component component)
            {
                value = component.GetComponent<T>();
                if (value == null)
                {
                    Debug.LogWarning($"Component '{component.GetType()}' on GameObject '{component.gameObject.name}' does not implement '{typeof(T)}'.", component);
                }
                return;
            }

            // Target does not implement T and is not a GameObject or Component
            Debug.LogWarning($"The assigned object '{_target.name}' does not implement '{typeof(T)}' and is not a GameObject or Component.", _target);
            value = null;
            _target = null;
        }
    }
}