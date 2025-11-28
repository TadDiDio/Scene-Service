using UnityEngine;

namespace SceneService
{
    public static class Scenes
    {
        private static SceneController _controller;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _controller = new SceneController(SceneMap.Active);
            Application.quitting += Shutdown;
        }

        private static void Shutdown()
        {
            _controller.Dispose();
            _controller = null;
        }
    }
}