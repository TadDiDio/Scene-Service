using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

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
        [SerializeField] private List<SceneReference> dependencies;

        [SerializeField] private List<string> tags;
        
        /// <summary>
        /// A readonly list of all dependencies.
        /// </summary>
        public IReadOnlyList<SceneReference> Dependencies => dependencies;
        
        /// <summary>
        /// A readonly list of all tags.
        /// </summary>
        public IReadOnlyList<string> Tags => tags;
        
        /// <summary>
        /// Gets a new list of all scenes in this group.
        /// </summary>
        /// <returns>The scene list.</returns>
        public List<SceneReference> All() => new(dependencies) { ActiveScene };
    }
}