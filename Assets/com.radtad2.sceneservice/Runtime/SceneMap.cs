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
        /// The scene to load first in single mode.
        /// </summary>
        public SceneReference BootstrapScene;

        /// <summary>
        /// Groups of related scenes to load as single units.
        /// </summary>
        public List<SceneGroup> Groups;

        private static SceneMap _cached;
        private static bool _loadedByGetter;

        private void Awake()
        {
            if (!_loadedByGetter || _cached)
                throw new InvalidOperationException(
                    $"You may only access the {nameof(SceneMap)} instance by {nameof(SceneMap)}.{nameof(Active)}). This is to avoid accidental disk modifications during runtime!");
        }

        public string Validate()
        {
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
                
                // Also check active scene against dependencies
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
                _loadedByGetter = true;

#if UNITY_EDITOR
                string guid = EditorPrefs.GetString(EditorKey, "");
                if (string.IsNullOrEmpty(guid)) throw new InvalidOperationException("SceneService: No SceneInfo selected in the Tools -> Scene Map Settings Window.");

                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SceneMap>(path);

                if (!asset) throw new InvalidOperationException($"SceneService: Selected SceneInfo asset at path '{path}' cannot be loaded.");
#else 
                // Build injector ensures there is only one.
                var asset = Resources.FindObjectsOfTypeAll<SceneMap>().FirstOrDefault();
#endif
                
                if (!asset) throw new InvalidOperationException("SceneService: No active SceneMap loaded. Make sure to set one in Tools -> Scene Map Settings");
                
                _cached = Instantiate(asset);
                _cached.hideFlags = HideFlags.DontSave;
                return _cached;
            }
        }

        public void Dispose()
        {
            _cached = null;
            _loadedByGetter = false;
        }
    }
}