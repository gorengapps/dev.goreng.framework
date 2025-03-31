using System.Threading.Tasks;
using UnityEngine;

namespace Frame.Runtime.Canvas
{
    public class AbstractCanvas: MonoBehaviour, ICanvas
    {
        public virtual Task SceneWillUnloadAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task SceneWillLoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}