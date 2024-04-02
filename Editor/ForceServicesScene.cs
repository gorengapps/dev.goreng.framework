#if UNITY_EDITOR 

using UnityEditor;
using UnityEditor.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Item allows you to force the service scene as the default scene, this could be a wanted function since the
    /// framework relies on the services scene.
    /// Note: This assumes that the first scene in your scenes list is the services scene.
    /// </summary>
    [InitializeOnLoad]
    public class ForceServicesScene
    {
        private const string _menuName = "Tools/Force Services Scene";
        private static bool _enabled = false;

        static ForceServicesScene()
        {
            _enabled = EditorPrefs.GetBool(_menuName, false);
            Menu.SetChecked(_menuName, _enabled);
        }
        
        private static void PerformToggle(bool enabled) {
            
            Menu.SetChecked(_menuName, enabled);
            EditorPrefs.SetBool(_menuName, enabled);

            _enabled = enabled;
            
            Force(_enabled);
        }
        
        private static void Force(bool force)
        {
            SceneAsset asset = null;
            
            if (force)
            {
                var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
                asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            }
            
            EditorSceneManager.playModeStartScene = asset;
        }
        
        [MenuItem(_menuName)]
        public static void ToggleForce()
        {
            // Delay the first call so the menu has time to populate
            EditorApplication.delayCall += () => {
                PerformToggle(!_enabled);
            };
        }
    }
}

#endif