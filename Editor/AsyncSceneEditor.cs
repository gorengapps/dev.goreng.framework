#if UNITY_EDITOR

using Frame.Runtime.Scene;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine.UIElements;

namespace Frame.Editor
{
    [CustomEditor(typeof(AsyncScene), true)]
    public class AsyncSceneEditor : UnityEditor.Editor
    {
        private const string _sceneKey = "scenes";
            
        private void UpdateAddressableLabel()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found. Please ensure Addressables is properly configured.");
                return;
            }
            
            var path = AssetDatabase.GetAssetPath(target);
            
            var assetGuid = AssetDatabase.AssetPathToGUID(path);
            
            // Addressable was not created for this item
            if (settings.FindAssetEntry(assetGuid) == null)
            {
                settings.CreateAssetReference(assetGuid);    
            }
            
            var entry = settings.FindAssetEntry(assetGuid);
            if (entry == null)
            {
                Debug.LogError($"Failed to create or find addressable entry for asset: {path}");
                return;
            }

            var sceneTypeProperty = serializedObject.FindProperty("_sceneType"); 
            
            // If scene type is empty, default to the name
            if(string.IsNullOrEmpty(sceneTypeProperty.stringValue) || sceneTypeProperty.stringValue != target.name) {
                sceneTypeProperty.stringValue = target.name;
            }
            
            if (!entry.labels.Contains(_sceneKey))
            {
                entry.SetLabel(_sceneKey, true, true, false);   
            }
            
            if (entry.address != path)
            {
                entry.SetAddress(path);
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            // Update the serializedProperty
            serializedObject.Update();
            
            UpdateAddressableLabel();
            serializedObject.ApplyModifiedProperties();
            return base.CreateInspectorGUI();
        }
        
        [MenuItem("Assets/Create/Framework/Scene/Create Scene", false, 1)]
        public static void CreateScene()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/";
            }
            
            var asset = CreateInstance<AsyncScene>();
            
            AssetDatabase.CreateAsset(asset, path + "/Scene.asset"); 
            EditorUtility.SetDirty(asset);
            
            AssetDatabase.SaveAssets();
            
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            
            settings.CreateAssetReference(assetGuid);
            Selection.activeObject = asset;
        } 
    }
}

#endif