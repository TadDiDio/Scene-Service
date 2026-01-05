using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SceneService
{
    /// <summary>
    /// Holds functionality to load, unload, and reload scenes.
    /// </summary>
    public interface ISceneController
    {
        /// <summary>
        /// Invoked when a scene group starts loading. Parameter is the name of the loading group.
        /// </summary>
        public event Action<string> OnLoadStart;
        
        /// <summary>
        /// Invoked when a scene group progresses in loading.
        /// </summary>
        public event Action<ProgressInfo> OnProgress;
        
        /// <summary>
        /// Invoked when a scene group finishes loading.
        /// </summary>
        public event Action<LoadCompleteInfo> OnLoadComplete;
        
        /// <summary>
        /// Gets a readonly list of tags associated with a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>The tags.</returns>
        public IReadOnlyList<string> GetTagsForGroup(string groupName);

        /// <summary>
        /// Loads a group of scenes.
        /// </summary>
        /// <param name="newGroup">The group to load.</param>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <param name="reloadPolicy">How to handle scenes that are needed in the next group but loaded in the current.</param>
        /// <returns>A task representing the async work.</returns>
        public Task LoadGroupAsync(SceneGroup newGroup, ISceneManager manager = null, ReloadPolicy reloadPolicy = ReloadPolicy.All);

        /// <summary>
        /// Loads a group of scenes.
        /// </summary>
        /// <param name="groupName">The group to load.</param>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <param name="reloadPolicy">How to handle scenes that are needed in the next group but loaded in the current.</param>
        /// <returns>A task representing the async work.</returns>
        public Task LoadGroupAsync(string groupName, ISceneManager manager = null, ReloadPolicy reloadPolicy = ReloadPolicy.All);

        /// <summary>
        /// Reloads the current group of scenes, removing any loaded extra scenes.
        /// </summary>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <returns>A task representing the async work.</returns>
        public Task ReloadActiveGroupAsync(ISceneManager manager = null);
  
        /// <summary>
        /// Loads a new extra scene additively.
        /// </summary>
        /// <param name="scenePath">The scene path to load.</param>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <returns>A task representing the async work.</returns>
        public Task LoadExtraSceneAsync(string scenePath, ISceneManager manager = null);

        /// <summary>
        /// Unloads an extra scene.
        /// </summary>
        /// <param name="scenePath">The scene to unload.</param>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <returns>A task representing the async work.</returns>
        public Task UnloadExtraSceneAsync(string scenePath, ISceneManager manager = null);
        
        /// <summary>
        /// Unloads all extra scenes.
        /// </summary>
        /// <param name="manager">The manager to use, Unity's if null.</param>
        /// <returns>A task representing the async work.</returns>
        public Task UnloadExtraScenesAsync(ISceneManager manager = null);
    }
}