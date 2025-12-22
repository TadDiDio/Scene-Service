using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SceneService
{
    /// <summary>
    /// Ensures that ONLY the editor selected scene map is loaded in the project.
    /// </summary>
    public class SceneServiceValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            // Find all scene map assets in the project
            string[] sceneMapGuids = AssetDatabase.FindAssets($"t:{nameof(SceneMap)}");
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            foreach (string mapGuid in sceneMapGuids)
            {
                string mapPath = AssetDatabase.GUIDToAssetPath(mapGuid);

                foreach (string assetPath in allAssets)
                {
                    // Skip the scene map inspecting itself
                    if (assetPath == mapPath) continue;

                    // Find all dependencies of this asset
                    var deps = AssetDatabase.GetDependencies(assetPath, true);

                    // If this asset references SceneMap, that's illegal
                    if (deps.Contains(mapPath))
                    {
                        throw new BuildFailedException(
                            $"Invalid reference detected!\n\n" +
                            $"The asset:\n  {assetPath}\n\n" +
                            $"contains a serialized reference to the SceneMap asset:\n  {mapPath}\n\n" +
                            $"SceneMap assets cannot be referenced anywhere in the project.\n" +
                            $"They may ONLY be selected from:\n  Tools -> Scene Map Settings\n\n" +
                            $"Fix: remove the reference from {assetPath}."
                        );
                    }
                }
            }

            string guid = EditorPrefs.GetString(SceneMap.EditorKey, "");
            if (string.IsNullOrEmpty(guid)) throw new BuildFailedException("SceneService: No SceneInfo selected in the Scene Service Settings Window.");

            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<SceneMap>(path);

            if (!asset) throw new BuildFailedException($"SceneService: Selected SceneInfo asset at path '{path}' cannot be loaded.");
            
            Debug.Log($"[Scene Service] '{asset.ConfigName}' {nameof(SceneMap)} injected into the build for bootstrapping.");
            
            
            if (asset.LoadBootstrapperFirst && UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(0) != asset.BootstrapScene.Path)
            {
                throw new BuildFailedException("You must set your bootstrap scene to be first in the active build profile when using Scene Service's bootstrapper mode.");
            }
        }
    }
}
