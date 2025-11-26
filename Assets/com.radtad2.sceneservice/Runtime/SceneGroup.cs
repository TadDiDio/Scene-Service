using System;
using System.Collections.Generic;
using Eflatun.SceneReference;

namespace SceneService
{
    [Serializable]
    public struct SceneGroup
    {
        public string GroupName;
        public List<SceneData> Scenes;
    }
    
    [Serializable]
    public class SceneData {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public bool Active;
    }
}