#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Frame.Editor
{
    /// <summary>
    /// Forces the first scene in the build settings to be the start scene when entering Play Mode.
    /// This is useful when a framework relies on a specific initialization scene.
    /// </summary>
    [InitializeOnLoad]
    public class ForceServicesScene
    {
        private const string _menuName = "Tools/Force Services Scene";
        private static bool _enabled;

        /// <summary>
        /// Static constructor called on editor load and script recompile.
        /// </summary>
        static ForceServicesScene()
        {
            // We delay the initialization to avoid a race condition where this code
            // runs before the EditorBuildSettings are fully loaded by Unity.
            EditorApplication.delayCall += Initialize;
        }

        /// <summary>
        /// Initializes the tool's state after the editor is ready.
        /// </summary>
        private static void Initialize()
        {
            _enabled = EditorPrefs.GetBool(_menuName, false);
            Menu.SetChecked(_menuName, _enabled);
            ApplyForceSetting(_enabled);
        }

        /// <summary>
        /// Toggles the forcing behavior on or off.
        /// </summary>
        [MenuItem(_menuName)]
        public static void ToggleForce()
        {
            // When a user clicks the menu, the editor is already loaded and idle,
            // so we can perform the action immediately.
            PerformToggle(!_enabled);
        }

        private static void PerformToggle(bool isEnabled) {
            Menu.SetChecked(_menuName, isEnabled);
            EditorPrefs.SetBool(_menuName, isEnabled);
            _enabled = isEnabled;
            ApplyForceSetting(isEnabled);
        }

        /// <summary>
        /// Applies the setting to the EditorSceneManager.
        /// </summary>
        private static void ApplyForceSetting(bool force)
        {
            if (!force)
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            if (EditorBuildSettings.scenes.Length > 0)
            {
                string pathOfFirstScene = EditorBuildSettings.scenes[0].path;
                SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
                EditorSceneManager.playModeStartScene = asset;
            }
            else
            {
                Debug.LogWarning("Force Services Scene: No scenes found in build settings. Cannot set play mode start scene.");
                // Ensure the setting is cleared if no scenes are available.
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}

#endif