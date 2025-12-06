#if UNITY_EDITOR

using Frame.Runtime.Scene;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
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
            
            var root = new VisualElement();
            root.Add(new HelpBox("The Scene Name is automatically synced with the filename.", HelpBoxMessageType.Info));
            root.Add(new IMGUIContainer(() => {
                serializedObject.Update();
                var iterator = serializedObject.GetIterator();
                var enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath || "_sceneType" == iterator.propertyPath))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                    enterChildren = false;
                }
                serializedObject.ApplyModifiedProperties();
            }));
            
            return root;
        }
        
        [MenuItem("Assets/Create/Framework/Scene/Create Scene", false, 1)]
        public static void CreateScene()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorUtility.DisplayDialog("Addressables Not Initialized", 
                    "Addressable Asset Settings not found.\nPlease initialize Addressables first via Window > Asset Management > Addressables > Groups.", "OK");
                return;
            }

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/";
            }
            else if (!AssetDatabase.IsValidFolder(path))
            {
                // If the selected object is a file, get its directory
                path = System.IO.Path.GetDirectoryName(path);
            }
            
            var asset = CreateInstance<AsyncScene>();
            
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(path, "Scene.asset"));
            AssetDatabase.CreateAsset(asset, assetPath); 
            EditorUtility.SetDirty(asset);
            
            AssetDatabase.SaveAssets();
            
            var createdAssetPath = AssetDatabase.GetAssetPath(asset);
            var assetGuid = AssetDatabase.AssetPathToGUID(createdAssetPath);
            
            settings.CreateAssetReference(assetGuid);
            
            Selection.activeObject = asset;
        } 
    }
}

#endif