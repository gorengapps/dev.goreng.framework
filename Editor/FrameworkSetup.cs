#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using Frame.Runtime.DI.Container;
using Frame.Runtime.RunLoop;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frame.Editor
{ 
    public class FrameworkSetup : EditorWindow
    {
        private const string _editorWindowPath = "Packages/dev.goreng.frame/Editor/UI/FrameworkWindow.uxml";
        private Button _runLoopButton;
        
        [MenuItem ("Framework/Setup")]
        public static void ShowSetup()
        {
            if (HasOpenInstances<FrameworkSetup>())
            {
                return;
            }
            
            var window = GetWindow<FrameworkSetup>();
            window.titleContent.text = "Framework Setup";
            window.ShowPopup();   
        }

        private DependenciesContainer GetDependencyContainer()
        {
            var container = AssetDatabase
                .FindAssets($"t:{nameof(DependenciesContainer)}")
                .Select(path =>
                    AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(path), typeof(DependenciesContainer)))
                .Cast<DependenciesContainer>()
                .FirstOrDefault();

            if (container == null)
            {
                return null;
            }
            else
            {
                return container;
            }
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_editorWindowPath);
            
            var uxml = visualTree.CloneTree();
            
            for(var i = 0; i < uxml.childCount; i++)
            {
                rootVisualElement.Add(uxml.ElementAt(i));
            }

            _runLoopButton = rootVisualElement.Q<Button>("RunLoopButton");
            _runLoopButton.clicked += OnRunLoopButtonPressed;
        }

        [CanBeNull]
        private GameObject GetRunLoop()
        {
            var runLoop = Resources
                .FindObjectsOfTypeAll<RunLoop>()
                .FirstOrDefault();

            return runLoop == null ? null : runLoop.gameObject;
        }
        
        private void OnRunLoopButtonPressed()
        {
            var runLoop = GetRunLoop();
            var container = GetDependencyContainer();

            if (container == null)
            {
                EditorUtility.DisplayDialog(
                    "Container does not exist", 
                    $"Please create a dependency container before running this", 
                    "Ok"
                );
            }
            
            if (runLoop != null)
            {
                EditorUtility.DisplayDialog(
                    "Runloop already exists", 
                    $"There is no need to create this anymore", 
                    "Ok"
                );

                Selection.activeObject = runLoop;
                return;
            }

            var obj = new GameObject("Runloop");
            obj.AddComponent<RunLoop>();
            
            string localPath = "Assets/Resources/Prefabs/Runloop.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath, out var prefabSuccess);
            
            DestroyImmediate(obj);
            
            if (!prefabSuccess)
            {
                return;
            }
            
            container.EditorRegisterSingleton(prefab.GetComponent<RunLoop>());
            
            AssetDatabase.SaveAssetIfDirty(container);
            Selection.activeObject = prefab;
        }
    }
}

#endif