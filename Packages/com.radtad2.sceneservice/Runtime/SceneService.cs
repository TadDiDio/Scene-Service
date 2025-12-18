using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SceneService
{
    public static class SceneService
    {
#if UNITY_EDITOR
        public readonly struct EditorSceneGroup
        {
            public readonly string ActivePath;
            public readonly IReadOnlyList<string> DependencyPaths;

            public EditorSceneGroup(string active, IReadOnlyList<string> dependencies)
            {
                ActivePath = active;
                DependencyPaths = dependencies;
            }
        }
        
        /// <summary>
        /// Represents the scenes loaded in the editor when edit mode was exited.
        /// </summary>
        /// <remarks>This can be used to perform custom behavior for editor workflows.</remarks>
        public static EditorSceneGroup EditorGroup;
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            var dependencies = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene == SceneManager.GetActiveScene()) continue;
                dependencies.Add(scene.path);
            }
            
            EditorGroup = new EditorSceneGroup(SceneManager.GetActiveScene().path, dependencies);
#endif
            if (SceneMap.Active.LoadBootstrapperFirst)
            {
                SceneManager.LoadScene(SceneMap.Active.BootstrapScene.Path, LoadSceneMode.Single);
            }
        }

        /// <summary>
        /// Gets a new scene controller.
        /// </summary>
        /// <returns>A new scene controller.</returns>
        public static SceneController BuildSceneController()
        {
            SceneLogger.Info($"Loaded SceneMap '{SceneMap.Active.ConfigName}'");
            return new SceneController(SceneMap.Active);
        }
    }
}