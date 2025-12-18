using SceneService.Unity;
using UnityEngine.SceneManagement;

namespace SceneService
{
    public class SimpleUnitySceneManager : ISceneManager
    {
        public ISceneLoadOperation LoadSceneAsync(string scenePath)
        {
            return new AsyncSceneLoadOperation(SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive));
        }

        public ISceneLoadOperation UnloadSceneAsync(string scenePath)
        {
            return new AsyncSceneLoadOperation(SceneManager.UnloadSceneAsync(scenePath));
        }

        public void SetActiveScene(string scenePath)
        {
            var scene = SceneManager.GetSceneByPath(scenePath);
            SceneManager.SetActiveScene(scene);
        }
    }
}