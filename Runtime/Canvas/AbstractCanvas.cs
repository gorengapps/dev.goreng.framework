using UnityEngine;

namespace Frame.Runtime.Canvas
{
    public class AbstractCanvas: MonoBehaviour, ICanvas
    {
        public virtual Awaitable SceneWillUnloadAsync()
        {
            return Awaitable.NextFrameAsync();
        }

        public virtual Awaitable SceneWillLoadAsync()
        {
            return Awaitable.NextFrameAsync();
        }
    }
}