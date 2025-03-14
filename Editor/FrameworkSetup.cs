#if UNITY_EDITOR

using System.Linq;
using Framework.DI.Container;
using Framework.Loop;
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
                .FindObjectsOfTypeAll<BaseRunLoop>()
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
                
                return;
            }
            
            if (runLoop != null)
            {
                Selection.activeObject = runLoop;
                    container.EditorRegisterSingleton(runLoop.GetComponent<BaseRunLoop>());
                AssetDatabase.SaveAssetIfDirty(container);
                return;
            }

            var obj = new GameObject("Runloop");
            obj.AddComponent<BaseRunLoop>();
            
            string localPath = "Assets/Resources/Prefabs/Runloop.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath, out var prefabSuccess);
            
            DestroyImmediate(obj);
            
            if (!prefabSuccess)
            {
                return;
            }
            
            container.EditorRegisterSingleton(prefab.GetComponent<BaseRunLoop>());
            
            AssetDatabase.SaveAssetIfDirty(container);
            Selection.activeObject = prefab;
        }
    }
}

#endif