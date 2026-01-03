using System.Collections.Generic;

namespace SceneService
{
    public static class Scenes
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