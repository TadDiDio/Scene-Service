using System;
using System.Collections.Generic;
using Eflatun.SceneReference;

namespace SceneService
{
    [Serializable]
    public class SceneGroup
    {
        /// <summary>
        /// The name of this group.
        /// </summary>
        public string GroupName;
        
        /// <summary>
        /// The main scene to load and set as active.
        /// </summary>
        public SceneReference ActiveScene;
        
        /// <summary>
        /// All scenes that the active scene depends on.
        /// </summary>
        public List<SceneReference> Dependencies;
    }
}