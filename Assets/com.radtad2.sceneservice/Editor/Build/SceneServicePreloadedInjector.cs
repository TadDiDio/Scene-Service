using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SceneService
{
    /// <summary>
    /// Ensures that the selected scene map is packed with the project dependencies despite not being referenced.
    /// </summary>
    public class SceneServicePreloadedInjector : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            string guid = EditorPrefs.GetString(SceneMap.EditorKey, "");
            if (string.IsNullOrEmpty(guid)) throw new BuildFailedException("SceneService: No SceneInfo selected in the Scene Service Settings Window.");

            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<SceneMap>(path);

            if (!asset) throw new BuildFailedException($"SceneService: Selected SceneInfo asset at path '{path}' cannot be loaded.");

            // Inject the selected asset into PreloadedAssets
            var preloaded = PlayerSettings.GetPreloadedAssets();
            if (!ArrayUtility.Contains(preloaded, asset))
            {
                ArrayUtility.Add(ref preloaded, asset);
                PlayerSettings.SetPreloadedAssets(preloaded);
            }
        }
    }
}