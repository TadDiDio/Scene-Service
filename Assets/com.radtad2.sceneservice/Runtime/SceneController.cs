using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneService
{
    public class SceneController : IDisposable
    {
        /// <summary>
        /// Invoked when a scene group begins loading.
        /// </summary>
        public event Action OnLoadStarted;
        
        /// <summary>
        /// Invoked when a load group progresses.
        /// </summary>
        public event Action<float> OnLoadProgressed;
        
        /// <summary>
        /// Invoked when a scene group completes loading.
        /// </summary>
        public event Action OnLoadCompleted;
        
        private const int PollIntervalMillis = 50;

        private bool _loadingGroup;
        
        private SceneMap _sceneMap;
        private SceneGroup _activeGroup;
        private List<SceneReference> _extraScenes = new();
        private ISceneManager _defaultManager = new UnitySceneManager();
        
        public SceneController(SceneMap sceneMap)
        {
            _sceneMap = sceneMap;
            SceneManager.LoadScene(_sceneMap.BootstrapScene.Path, LoadSceneMode.Single);
        }

        /// <summary>
        /// Loads a group of scenes additively and unloads the currently active group if there is one.
        /// </summary>
        /// <param name="newGroup">The group to load.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="reloadCommonDependencies">Whether to reload dependencies of this group that are already loaded.</param>
        public async Task LoadGroupAsync(SceneGroup newGroup, ISceneManager overrideManager = null, bool reloadCommonDependencies = false)
        {
            if (_loadingGroup)
            {
                Debug.LogError("Attempted to load a group while one was already loading. This is not allowed.");
                return;
            }

            if (!_sceneMap.Groups.Contains(newGroup))
            {
                Debug.LogWarning($"Attempted to load scene group '{newGroup.GroupName}' but it does not exist in the scene map.");
                return;
            }
            
            try
            {
                _loadingGroup = true;
             
                var manager = overrideManager ?? _defaultManager;

                if (newGroup == _activeGroup)
                {
                    await ReloadActiveGroupAsync(manager, !reloadCommonDependencies);
                    return;
                }

                // Scenes to load
                var scenesToLoad = new List<SceneReference>(newGroup.Dependencies) { newGroup.ActiveScene };
                
                // Currently loaded dependencies
                var loadedDependencies = _activeGroup.Dependencies.ToList();
                loadedDependencies.AddRange(_extraScenes);
                loadedDependencies = loadedDependencies.Where(r => r.LoadedScene.isLoaded).ToList();

                // Scenes to unload
                var scenesToUnload = new List<SceneReference>(loadedDependencies);
                if (_activeGroup.ActiveScene.LoadedScene.isLoaded) scenesToUnload.Add(_activeGroup.ActiveScene);
                
                if (!reloadCommonDependencies)
                {
                    var commonLoadedDependencies = new List<SceneReference>();
                    foreach (var loaded in loadedDependencies)
                    {
                        if (newGroup.Dependencies.Any(toLoad => toLoad.Guid == loaded.Guid))
                        {
                            commonLoadedDependencies.Add(loaded);
                        }
                    }

                    foreach (var common in commonLoadedDependencies)
                    {
                        scenesToLoad.Remove(common);
                        scenesToUnload.Remove(common);
                    }
                }
                
                await UnloadScenesAsync(scenesToUnload, manager);

                OnLoadStarted?.Invoke();
                var operation = new SceneOperationGroup();
                foreach (var scene in scenesToLoad)
                {
                    operation.AddOperation(manager.LoadSceneAsync(scene.Path));
                }

                while (!operation.IsDone)
                {
                    OnLoadProgressed?.Invoke(operation.Progress);
                    await Task.Delay(PollIntervalMillis);
                }
                
                _activeGroup = newGroup;
                OnLoadCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _loadingGroup = false;
            }
        }
        
        /// <summary>
        /// Loads a group of scenes additively and unloads the currently active group if there is one.
        /// </summary>
        /// <param name="groupName">The group to load.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="reloadCommonDependencies">Whether to reload dependencies of this group that are already loaded.</param>
        public async Task LoadGroupAsync(string groupName, ISceneManager overrideManager = null, bool reloadCommonDependencies = false)
        {
            var group = _sceneMap.Groups.FirstOrDefault(g => g.GroupName == groupName);

            if (group == null) throw new ArgumentException($"No scene group with the name {groupName} was found.");
            
            await LoadGroupAsync(group, overrideManager, reloadCommonDependencies);
        }

        /// <summary>
        /// Adds a scene to the currently active scene group.
        /// </summary>
        /// <param name="scenePath">The scene to add.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public async Task AddToActiveGroupAsync(string scenePath, ISceneManager overrideManager = null)
        {
            var manager = overrideManager ?? _defaultManager;

            var operation = manager.LoadSceneAsync(scenePath);
            while (!operation.IsDone())
            {
                await Task.Delay(PollIntervalMillis);
            }

            _extraScenes.Add(new SceneReference(scenePath));
        }

        /// <summary>
        /// Adds a scene to the currently active scene group.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public async Task AddToActiveGroupAsync(SceneReference scene, ISceneManager overrideManager = null)
        {
            await AddToActiveGroupAsync(scene.Path, overrideManager);
        }

        /// <summary>
        /// Reloads the active scene group.
        /// </summary>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="reloadOnlyActiveScene">Whether to reload only the active scene or all dependencies as well.</param>
        public async Task ReloadActiveGroupAsync(ISceneManager overrideManager = null, bool reloadOnlyActiveScene = false)
        {
            var scenes = new List<SceneReference>();
            if (reloadOnlyActiveScene)
            {
                scenes.Add(_activeGroup.ActiveScene);   
            }
            else
            {
                scenes = new List<SceneReference>(_activeGroup.Dependencies) { _activeGroup.ActiveScene};
            }
            
            await UnloadScenesAsync(scenes, overrideManager);
            await LoadGroupAsync(_activeGroup, overrideManager, true);
        }

        /// <summary>
        /// Unloads all scenes except for the bootstrapper and the active scene.
        /// </summary>
        /// <remarks>If you want to unload all scenes before loading another group, simply call LoadGroupAsync.</remarks>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public async Task UnloadGroupDependenciesAsync(ISceneManager overrideManager = null)
        {
            
        }

        /// <summary>
        /// Removes a scene from the active group. This cannot be the active scene.
        /// </summary>
        /// <param name="scenePath">The scene to remove.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public async Task RemoveFromActiveGroupAsync(string scenePath, ISceneManager overrideManager = null)
        {
            
        }

        /// <summary>
        /// Removes a scene from the active group. This cannot be the active scene.
        /// </summary>
        /// <param name="scene">The scene to remove.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>s
        public async Task RemoveFromActiveGroupAsync(SceneReference scene, ISceneManager overrideManager = null)
        {
            await RemoveFromActiveGroupAsync(scene.Path, overrideManager);
        }

        private async Task UnloadScenesAsync(IEnumerable<SceneReference> scenes, ISceneManager overrideManager = null)
        {
            await UnloadScenesAsync(scenes.Select(r => r.Path), overrideManager);
        }
        
        private async Task UnloadScenesAsync(IEnumerable<string> scenePaths, ISceneManager overrideManager = null)
        {
            var manager = overrideManager ?? _defaultManager;
            
            foreach (var scenePath in scenePaths)
            {
                
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}