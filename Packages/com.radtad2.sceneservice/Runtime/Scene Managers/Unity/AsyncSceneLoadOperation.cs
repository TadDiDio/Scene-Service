using UnityEngine;

namespace SceneService.Unity
{
    public class AsyncSceneLoadOperation : ISceneLoadOperation
    {
        private AsyncOperation _operation;
        
        public AsyncSceneLoadOperation(AsyncOperation operation) => _operation = operation;
        public bool IsDone() => _operation.isDone;
        public float Progress() => _operation.progress;
    }
}