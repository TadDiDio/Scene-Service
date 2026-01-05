using System.Collections.Generic;
using Eflatun.SceneReference.Editor;
using SceneService;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorInitializer
    {
        static EditorInitializer()
        {
            SceneDataMapsGenerator.Run(true);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is not PlayModeStateChange.ExitingEditMode) return;
            
            var dependencies = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene == SceneManager.GetActiveScene()) continue;
                dependencies.Add(scene.path);
            }
            
            Scenes.EditorGroup = new Scenes.EditorSceneGroup(SceneManager.GetActiveScene().path, dependencies);
            
            var sceneMap = SceneMap.Active;
            if (sceneMap && sceneMap.LoadBootstrapperFirst)
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneMap.BootstrapScene.Path);
            }
            else EditorSceneManager.playModeStartScene = null;
        }
    }
}