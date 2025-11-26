using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneService
{
    public static class SceneController
    {
        private static SceneMap _info;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _info = Resources.FindObjectsOfTypeAll<SceneMap>().FirstOrDefault();
            
            if (!_info) throw new System.Exception("Could not find an active ProjectScenesInfo. Project cannot boot! Go to Tools -> Project Scenes Info to set an asset.");
            
            SceneManager.LoadScene(_info.BootstrapScene.Path, LoadSceneMode.Single);
        }
    }
}