using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SceneService
{
    public class SceneController : ISceneController
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
        
        private bool _isLoadingGroup;
        private SceneMap _sceneMap;
        private SceneGroup _activeGroup;
        private List<string> _extraScenesPaths = new();

        private SimpleUnitySceneManager _defaultManager = new();
        
        public SceneController(SceneMap sceneMap)
        {
            _sceneMap = sceneMap;
        }

        public IReadOnlyList<string> GetTagsForGroup(string groupName)
        {
            return _sceneMap.Groups.FirstOrDefault(g => g.GroupName == groupName)?.Tags ?? Array.Empty<string>();   
        }

        public async Task LoadGroupAsync(SceneGroup newGroup, ISceneManager manager = null, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            manager ??= _defaultManager;
            
            if (_isLoadingGroup)
            {
                SceneLogger.Error("Attempted to load a group while one was already loading. This is not allowed and will be cancelled.");
                return;
            }

            if (!_sceneMap.Groups.Contains(newGroup))
            {
                SceneLogger.Warning($"Attempted to load scene group '{newGroup.GroupName}' but it does not exist in the scene map.");
                return;
            }
            try
            {
                _isLoadingGroup = true;
                OnLoadStart?.Invoke(newGroup.GroupName);
                
                // Group scenes by operation
                var dependenciesToLoad = newGroup.Dependencies.Select(r => r.Path).ToList();

                if (_activeGroup != null)
                {
                    var dependenciesToUnload = new List<string>();
                    void FillUnloadList(IEnumerable<string> paths, bool forceReload)
                    {
                        foreach (var path in paths)
                        {
                            bool isNeeded = dependenciesToLoad.Contains(path);
                            
                            if (!isNeeded || forceReload) dependenciesToUnload.Add(path);
                            else dependenciesToLoad.Remove(path);
                        }
                    }
                    
                    // We guarantee that the active scene is unloaded first, followed by the active scene.
                    
                    reloadPolicy.Decompose(out var reloadActive, out var reloadDependencies, out var reloadExtras);
                    
                    bool activeIsDependency = dependenciesToLoad.Contains(_activeGroup.ActiveScene.Path);

                    if (!activeIsDependency || reloadActive)
                    {
                        await UnloadSceneAsync(_activeGroup.ActiveScene.Path, manager);
                    }
                    
                    FillUnloadList(_activeGroup.Dependencies.Select(r => r.Path), reloadDependencies);
                    FillUnloadList(_extraScenesPaths, reloadExtras);
                    
                    await UnloadScenesAsync(dependenciesToUnload, manager);
                    _extraScenesPaths.Clear();
                }
                // In case the editor loaded extra scenes for testing, unload them all
                else if (_extraScenesPaths.Count > 0)
                {
                    await UnloadScenesAsync(_extraScenesPaths, manager);
                }
                
                // We guarantee that dependencies load first, followed by the active scene.
                
                SceneOperationGroup dependencyOperations = new();
                foreach (var path in dependenciesToLoad)
                {
                    dependencyOperations.AddOperation(manager.LoadSceneAsync(path));
                }

                while (!dependencyOperations.IsDone)
                {
                    var progress = dependencyOperations.Progress * 0.8f;
                    OnProgress?.Invoke(new ProgressInfo(newGroup.GroupName, progress));
                    await Task.Yield();
                }

                var activeOperation = manager.LoadSceneAsync(newGroup.ActiveScene.Path);
                while (!activeOperation.IsDone())
                {
                    var progress = 0.8f + activeOperation.Progress() * 0.2f;
                    OnProgress?.Invoke(new ProgressInfo(newGroup.GroupName, progress));
                    await Task.Yield();
                }
                
                manager.SetActiveScene(newGroup.ActiveScene.Path);
                _activeGroup = newGroup;
                OnLoadComplete?.Invoke(new LoadCompleteInfo(newGroup.GroupName, true));
            }
            catch (Exception e)
            {
                SceneLogger.Error(e.ToString());
                OnLoadComplete?.Invoke(new LoadCompleteInfo(newGroup.GroupName, false));
            }
            finally
            {
                _isLoadingGroup = false;
            }
        }
        
        public async Task LoadGroupAsync(string groupName, ISceneManager manager = null, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            int count = _sceneMap.Groups.Count(g => g.GroupName == groupName); 
            if (count != 1)
            {
                if (count == 0) throw new ArgumentException($"No scene group with the name {groupName} was found. Make sure it exists in Tools -> Scene Map Settings.");
                throw new ArgumentException($"More than one group with the name {groupName} was found in scene map. These should be unique.");
            }
            
            var group = _sceneMap.Groups.FirstOrDefault(g => g.GroupName == groupName);
            await LoadGroupAsync(group, manager, reloadPolicy);
        }

        public async Task ReloadActiveGroupAsync(ISceneManager manager = null)
        {
            await LoadGroupAsync(_activeGroup, manager);
        }
        
        public async Task LoadExtraSceneAsync(string scenePath, ISceneManager manager = null)
        {
            manager ??= _defaultManager;

            if (_extraScenesPaths.Contains(scenePath) ||
                _activeGroup.Dependencies.Any(r => r.Path == scenePath))
            {
                SceneLogger.Error($"Cannot load the same scene twice! '{scenePath}' is already loaded.");
                return;
            }
            
            var operation = manager.LoadSceneAsync(scenePath);
            while (!operation.IsDone()) await Task.Yield();
            _extraScenesPaths.Add(scenePath);
        }

        public async Task UnloadExtraSceneAsync(string scenePath, ISceneManager manager = null)
        {
            manager ??= _defaultManager;

            if (_extraScenesPaths.All(p => p != scenePath)) return;
            
            var operation = manager.UnloadSceneAsync(scenePath);
            while (!operation.IsDone()) await Task.Yield();
            _extraScenesPaths.Remove(scenePath);
        }

        public async Task UnloadExtraScenesAsync(ISceneManager manager = null)
        {
            await UnloadScenesAsync(_extraScenesPaths, manager);
            _extraScenesPaths.Clear();
        }
        
        private async Task UnloadSceneAsync(string scenePath, ISceneManager manager = null)
        {
            manager ??= _defaultManager;

            var operation = manager.UnloadSceneAsync(scenePath);
            
            while (!operation.IsDone()) await Task.Yield();
        }
        
        private async Task UnloadScenesAsync(IEnumerable<string> scenePaths, ISceneManager manager = null)
        {
            manager ??= _defaultManager;

            var operation = new SceneOperationGroup();
            foreach (var scenePath in scenePaths)
            {
                operation.AddOperation(manager.UnloadSceneAsync(scenePath));
            }
            
            while (!operation.IsDone) await Task.Yield();
        }
    }
}