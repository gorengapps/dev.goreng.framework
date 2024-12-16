using System;

namespace Frame.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SceneAttribute : Attribute
    {
        public string sceneName { get; }

        public SceneAttribute(string sceneName)
        {
            this.sceneName = sceneName;
        }
    }
}