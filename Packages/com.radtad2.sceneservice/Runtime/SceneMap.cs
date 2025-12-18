using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#else
using System.Linq;
#endif

namespace SceneService
{
    [CreateAssetMenu(fileName = "SceneMap", menuName = "Scene Map", order = 0)]
    public class SceneMap : ScriptableObject, IDisposable
    {
        /// <summary>
        /// Key to get the active path from editor prefs.
        /// </summary>
        public const string EditorKey = "SceneMap.EditorKey";

        /// <summary>
        /// The name of this scene map.
        /// </summary>
        public string ConfigName = "Unnamed";

        /// <summary>
        /// Whether to load the bootstrapper first or not.
        /// </summary>
        public bool LoadBootstrapperFirst = true;
        
        /// <summary>
        /// The scene to load first in single mode.
        /// </summary>
        public SceneReference BootstrapScene;

        /// <summary>
        /// Groups of related scenes to load as single units.
        /// </summary>
        public List<SceneGroup> Groups;

        private static SceneMap _cached;

        public string Validate()
        {
            if (Groups == null) return string.Empty;

            if (BootstrapScene.UnsafeReason is not SceneReferenceUnsafeReason.None && LoadBootstrapperFirst)
                return "[SceneMap] Must reference a bootstrap scene.";
            
            foreach (var group in Groups)
            {
                var set = new HashSet<string>();
                
                // Fix duplicates in Dependencies
                for (int i = group.Dependencies.Count - 1; i >= 0; i--)
                {
                    var scene = group.Dependencies[i];

                    // Not set
                    if (scene.State is SceneReferenceState.Unsafe) continue;
                    
                    if (!set.Add(scene.Guid))
                    {
                        return $"[SceneMap] Duplicate scene '{scene.Name}' found in group '{group.GroupName}'";
                    }
                }
                
                // Check bootstrapper scene against active scene
                if (BootstrapScene.State is not SceneReferenceState.Unsafe &&
                    group.ActiveScene.State is not SceneReferenceState.Unsafe &&
                    BootstrapScene.Guid == group.ActiveScene.Guid)
                {
                    return $"[SceneMap] Bootstrap scene '{BootstrapScene.Name}' "
                           + $"cannot be the active scene for group '{group.GroupName}'";
                }
                
                // Check bootstrapper scene against dependencies
                if (BootstrapScene.State is not SceneReferenceState.Unsafe && set.Contains(BootstrapScene.Guid))
                {
                    return $"[SceneMap] Bootstrap scene '{BootstrapScene.Name}' "
                           + $"appeared in Dependencies for group '{group.GroupName}'";
                }
                
                // Check active scene against dependencies
                if (group.ActiveScene.State is not SceneReferenceState.Unsafe && set.Contains(group.ActiveScene.Guid))
                {
                    return $"[SceneMap] Active scene '{group.ActiveScene.Name}' "
                           + $"appeared in Dependencies for group '{group.GroupName}'";
                }
            }

            return string.Empty;
        }

        public static SceneMap Active
        {
            get
            {
                if (_cached) return _cached;

#if UNITY_EDITOR
                string guid = EditorPrefs.GetString(EditorKey, "");
                if (string.IsNullOrEmpty(guid)) throw new InvalidOperationException($"No {nameof(SceneMap)} selected. Make sure to set one int Tools -> Scene Map Settings.");

                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SceneMap>(path);

                if (!asset) throw new InvalidOperationException($"Selected {nameof(SceneMap)} asset at path '{path}' cannot be loaded. Make sure to set one int Tools -> Scene Map Settings.");
#else 
                // Build injector ensures there is only one.
                var asset = Resources.FindObjectsOfTypeAll<SceneMap>().FirstOrDefault();
#endif
                
                if (!asset) throw new InvalidOperationException($"No active {nameof(SceneMap)} loaded. Make sure to set one in Tools -> Scene Map Settings.");

                var error = asset.Validate();

                if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException(error);
                
                _cached = asset;
                
                return asset;
            }
        }

        public void Dispose()
        {
            _cached = null;
        }
    }
}