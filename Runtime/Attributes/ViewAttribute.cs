using System;
using JetBrains.Annotations;

namespace Frame.Runtime
{
    /// <summary>
    /// Attribute used to automatically fetch and inject view instances from the current scene.
    /// When applied to a field in a bootstrap class, the framework will automatically
    /// find and assign the corresponding view instance during scene initialization.
    /// </summary>
    /// <example>
    /// <code>
    /// [FetchView] private ISampleView _sampleView;
    /// </code>
    /// </example>
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public class FetchViewAttribute: Attribute
    {
        
    }
}