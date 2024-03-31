using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frame.Runtime.RunLoop
{
    public partial class RunLoop: MonoBehaviour
    {
        private readonly List<Action<float>> _subscribers = new List<Action<float>>();
        
        private void Update()
        {
            _subscribers.ForEach(x => x?.Invoke(Time.deltaTime));
        }
    }
    
    public partial class RunLoop: IRunLoop
    {
        public void Subscribe(Action<float> callback)
        {
            if (_subscribers.Contains(callback))
            {
                return;
            }
            
            _subscribers.Add(callback);
        }

        public void UnSubscribe(Action<float> callback)
        {
            _subscribers.Remove(callback);
        }

        public void Coroutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}