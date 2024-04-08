#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frame.Runtime.Scene.Editor
{
    [CustomEditor(typeof(AsyncScene), true)]
    public class AsyncSceneEditor : UnityEditor.Editor
    {
        private void UpdateAddressableLabel()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var path = AssetDatabase.GetAssetPath(target);
            
            var assetGuid = AssetDatabase.AssetPathToGUID(path);
            

            // Addressable was not created for this item
            if (settings.FindAssetEntry(assetGuid) == null)
            {
                settings.CreateAssetReference(assetGuid);    
            }
            
            var entry = settings.FindAssetEntry(assetGuid);

            if (entry.address != path)
            {
                entry.SetAddress(path);
            }
            
            // /entry.SetAddress();
            entry.SetLabel("scenes", true, true, false);

        }
        
        public override VisualElement CreateInspectorGUI()
        {
            UpdateAddressableLabel();
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