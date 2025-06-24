using System;

namespace Frame.Runtime.Attributes
{
    /// <summary>
    /// Attribute used to associate a bootstrap class with a specific scene name.
    /// This attribute is used by the navigation system to map scene identifiers
    /// to their corresponding bootstrap classes for automated scene loading.
    /// </summary>
    /// <example>
    /// <code>
    /// [Scene("MainMenuScene")]
    /// public class MainMenuBootstrap : AbstractBootstrap
    /// {
    ///     // Bootstrap implementation
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SceneAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the scene associated with this bootstrap class.
        /// </summary>
        public string sceneName { get; }

        /// <summary>
        /// Initializes a new instance of the SceneAttribute with the specified scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to associate with the bootstrap class.</param>
        public SceneAttribute(string sceneName)
        {
            this.sceneName = sceneName;
        }
    }
}