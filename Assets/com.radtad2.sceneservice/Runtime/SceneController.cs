using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneService
{
    public class SceneController
    {
        internal event Action OnLoadStarted;
        internal event Action<float> OnLoadProgressed;
        internal event Action OnLoadCompleted;
        
        private bool _isLoadingGroup;
        private SceneMap _sceneMap;
        private SceneGroup _activeGroup;
        private List<SceneReference> _extraScenes = new();
        
        internal SceneController(SceneMap sceneMap)
        {
            _sceneMap = sceneMap;
        }

        internal void Bootstrap()
        {
            SceneManager.LoadScene(_sceneMap.BootstrapScene.Path, LoadSceneMode.Single);
        }
        
        internal async Task LoadGroupAsync(SceneGroup newGroup, ISceneManager manager, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            if (_isLoadingGroup)
            {
                Debug.LogError("Attempted to load a group while one was already loading. This is not allowed and will be cancelled.");
                return;
            }

            if (!_sceneMap.Groups.Contains(newGroup))
            {
                Debug.LogWarning($"Attempted to load scene group '{newGroup.GroupName}' but it does not exist in the scene map.");
                return;
            }
            
            try
            {
                _isLoadingGroup = true;

                // Group scenes by operation
                var scenesToLoad = newGroup.All();
                var scenesToUnload = _activeGroup.Dependencies.Concat(_extraScenes).Append(_activeGroup.ActiveScene).ToList();
                
                reloadPolicy.Decompose(out var active, out var dependencies, out var extras);

                void RemoveIfNotReloaded(IEnumerable<SceneReference> scenes, bool shouldReload)
                {
                    if (shouldReload) return;
                    
                    foreach (var scene in scenes)
                    {
                        if (!scene.IsSafeAndLoaded()) continue;
                        if (!scenesToLoad.ContainsByGuid(scene)) continue;

                        scenesToLoad.RemoveByGuid(scene);
                        scenesToUnload.RemoveByGuid(scene);
                    }
                }
                
                RemoveIfNotReloaded(new[] { _activeGroup.ActiveScene }, active);
                RemoveIfNotReloaded(_activeGroup.Dependencies, dependencies);
                RemoveIfNotReloaded(_extraScenes, extras);
                
                // Unload scenes
                await UnloadScenesAsync(scenesToUnload, manager);

                // Load scenes
                OnLoadStarted?.Invoke();
                var operation = new SceneOperationGroup();
                foreach (var scene in scenesToLoad)
                {
                    operation.AddOperation(manager.LoadSceneAsync(scene.Path));
                }

                while (!operation.IsDone)
                {
                    OnLoadProgressed?.Invoke(operation.Progress);
                    await Task.Yield();
                }
                
                _activeGroup = newGroup;
                manager.SetActiveScene(_activeGroup.ActiveScene.Path);
                OnLoadCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _isLoadingGroup = false;
            }
        }
        
        internal async Task LoadGroupAsync(string groupName, ISceneManager manager, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            int count = _sceneMap.Groups.Count(g => g.GroupName == groupName); 
            if (count != 1)
            {
                if (count == 0) throw new ArgumentException($"No scene group with the name {groupName} was found.");
                throw new ArgumentException($"More than one group with the name {groupName} was found in scene map. These should be unique.");
            }
            
            var group = _sceneMap.Groups.FirstOrDefault(g => g.GroupName == groupName);
            await LoadGroupAsync(group, manager, reloadPolicy);
        }

        internal async Task LoadSceneAsync(string scenePath, ISceneManager manager)
        {
            var operation = manager.LoadSceneAsync(scenePath);
            while (!operation.IsDone()) await Task.Yield();

            _extraScenes.Add(new SceneReference(scenePath));
        }

        internal async Task ReloadActiveGroupAsync(ISceneManager manager, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            var scenesToUnload = new List<SceneReference>();
            reloadPolicy.Decompose(out var active, out var dependencies, out var extras);

            if (active) scenesToUnload.Add(_activeGroup.ActiveScene);
            if (dependencies) scenesToUnload.AddRange(_activeGroup.Dependencies);
            if (extras) scenesToUnload.AddRange(_extraScenes);
            
            await UnloadScenesAsync(scenesToUnload, manager);
            await LoadGroupAsync(_activeGroup, manager, ReloadPolicy.None);
        }

        internal async Task UnloadGroupDependenciesAsync(ISceneManager manager)
        {
            await UnloadScenesAsync(_activeGroup.Dependencies, manager);
        }

        internal async Task UnloadExtrasAsync(ISceneManager manager)
        {
            await UnloadScenesAsync(_extraScenes, manager);
        }
        
        internal async Task UnloadDependencyAsync(string scenePath, ISceneManager manager)
        {
            if (_activeGroup.Dependencies.All(r => r.Path != scenePath)) return;
            
            var operation = manager.UnloadSceneAsync(scenePath);
            while (!operation.IsDone()) await Task.Yield();
        }
        
        internal async Task UnloadExtraAsync(string scenePath, ISceneManager manager)
        {
            if (_extraScenes.All(r => r.Path != scenePath)) return;
            
            var operation = manager.UnloadSceneAsync(scenePath);
            while (!operation.IsDone()) await Task.Yield();
        }

        private static async Task UnloadScenesAsync(IEnumerable<SceneReference> scenes, ISceneManager manager)
        {
            await UnloadScenesAsync(scenes.Select(r => r.Path), manager);
        }
        
        private static async Task UnloadScenesAsync(IEnumerable<string> scenePaths, ISceneManager manager)
        {
            var operation = new SceneOperationGroup();
            foreach (var scenePath in scenePaths)
            {
                operation.AddOperation(manager.UnloadSceneAsync(scenePath));
            }
            
            while (!operation.IsDone) await Task.Yield();
        }
    }
}