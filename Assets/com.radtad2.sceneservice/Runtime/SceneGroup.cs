using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets a new list of all scenes in this group.
        /// </summary>
        /// <returns>The scene list.</returns>
        public List<SceneReference> All() => new(Dependencies) { ActiveScene };
    }
}