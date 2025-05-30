#if UNITY_EDITOR

using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Frame.Runtime.Attributes;
using Frame.Runtime.Bootstrap;

namespace Frame.Runtime
{
   public static class SceneMappingCodeGen 
   {
        private const string _templatePath = "Packages/dev.goreng.frame/Editor/Templates/SceneMapping.cs.template";
        private const string _outputPath   = "Assets/Scripts/Generated/Runtime/SceneMappings.g.cs";

        [MenuItem("Framework/Generate SceneMappings")]
        public static void GenerateFromTemplate()
        {
            if (!File.Exists(_templatePath))
            {
                Debug.LogError($"Template not found at {_templatePath}");
                return;
            }

            var template = File.ReadAllText(_templatePath);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.Contains("Unity") && !a.FullName.Contains("System"));

            var entries = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract
                            && typeof(IBootstrap).IsAssignableFrom(t)
                            && t.GetCustomAttribute<SceneAttribute>() != null)
                .Select(t =>
                {

                    var type = t.GetInterfaces()
                        .FirstOrDefault(x => x != typeof(IBootstrap)) ?? t;
                    
                    var sceneName = t.GetCustomAttribute<SceneAttribute>().sceneName;
                    return $"{{ \"{sceneName}\", typeof({type.FullName}) }},\n";
                })
                .OrderBy(line => line)
                .ToList();
            
            var generated = string.Join(Environment.NewLine,    entries);
            var outputText = template.Replace("%@", generated);
            
            var dir = Path.GetDirectoryName(_outputPath);
            
            if (!Directory.Exists(dir))
            {
                Debug.Assert(dir != null, nameof(dir) + " != null");
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllText(_outputPath, outputText);
            
            AssetDatabase.Refresh();
        } 
   }
}
#endif
