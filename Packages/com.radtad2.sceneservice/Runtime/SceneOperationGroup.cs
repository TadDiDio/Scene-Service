using System.Collections.Generic;
using System.Linq;

namespace SceneService
{
    public class SceneOperationGroup
    {
        public bool IsDone => _operations.All(o => o.IsDone());
        public float Progress => _operations.Average(o => o.Progress());
        
        private List<ISceneLoadOperation> _operations = new();

        public void AddOperation(ISceneLoadOperation operation)
        {
            _operations.Add(operation);
        }
    }
}