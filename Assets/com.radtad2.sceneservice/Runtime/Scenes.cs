using System;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;

namespace SceneService
{
    public static class Scenes
    {
        /// <summary>
        /// Invoked when a scene group begins loading.
        /// </summary>
        public static event Action OnLoadStarted;

        /// <summary>
        /// Invoked when a load group progresses.
        /// </summary>
        public static event Action<float> OnLoadProgressed;
        
        /// <summary>
        /// Invoked when a scene group completes loading.
        /// </summary>
        public static event Action OnLoadCompleted;
        
        private static SceneController _controller;
        private static Action _onLoadStarted = () => OnLoadStarted?.Invoke();
        private static Action<float> _onLoadProgressed = p => OnLoadProgressed?.Invoke(p);
        private static Action _onLoadCompleted = () => OnLoadCompleted?.Invoke();
        private static ISceneManager _defaultManager = new SimpleUnitySceneManager();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Application.quitting += Shutdown;
            
            _controller = new SceneController(SceneMap.Active);
            _controller.OnLoadStarted += _onLoadStarted;
            _controller.OnLoadProgressed += _onLoadProgressed;
            _controller.OnLoadCompleted += _onLoadCompleted;
            
            _controller.Bootstrap();
        }

        private static void Shutdown()
        {
            if (_controller == null) return;

            _controller.OnLoadStarted -= _onLoadStarted;
            _controller.OnLoadProgressed -= _onLoadProgressed;
            _controller.OnLoadCompleted -= _onLoadCompleted;
            _controller = null;
        }

        private static void Verify()
        {
            if (_controller == null) throw new InvalidOperationException("Cannot access scene API during or before RuntimeInitializeLoadType.BeforeSceneLoad.");
        }

        /// <summary>
        /// Loads a group of scenes additively and unloads the currently active group if there is one.
        /// </summary>
        /// <param name="newGroup">The group to load.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="reloadPolicy">How to handle scenes that are currently loaded but also needed by newGroup.</param>
        public static async Task LoadGroupAsync(SceneGroup newGroup, ISceneManager overrideManager = null,  ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            Verify();
            
            await _controller.LoadGroupAsync(newGroup, overrideManager ?? _defaultManager, reloadPolicy);
        }
        
        /// <summary>
        /// Loads a group of scenes additively and unloads the currently active group if there is one.
        /// </summary>
        /// <param name="groupName">The group to load.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="reloadPolicy">How to handle scenes that are currently loaded but also needed by newGroup.</param>
        public static async Task LoadGroupAsync(string groupName, ISceneManager overrideManager = null, ReloadPolicy reloadPolicy = ReloadPolicy.All)
        {
            Verify();

            await _controller.LoadGroupAsync(groupName, overrideManager ?? _defaultManager, reloadPolicy);
        }
        
        /// <summary>
        /// Adds a scene to the currently active scene group.
        /// </summary>
        /// <param name="scenePath">The scene to add.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public static async Task LoadSceneAsync(string scenePath, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.LoadSceneAsync(scenePath, overrideManager ?? _defaultManager);
        }
        
        /// <summary>
        /// Adds a scene to the currently active scene group.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        public static async Task LoadSceneAsync(SceneReference scene, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.LoadSceneAsync(scene.Path, overrideManager ?? _defaultManager);
        }

        /// <summary>
        /// Reloads the active scene group.
        /// </summary>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be loaded.</param>
        /// <param name="policy"></param>
        public static async Task ReloadActiveGroupAsync(ISceneManager overrideManager = null, ReloadPolicy policy = ReloadPolicy.All)
        {
            Verify();

            await _controller.ReloadActiveGroupAsync(overrideManager ?? _defaultManager, policy);
        }

        /// <summary>
        /// Unloads all dependencies of the current active scene group.
        /// </summary>
        /// <remarks>If you want to unload all scenes before loading another group, simply call LoadGroupAsync.</remarks>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadGroupDependenciesAsync(ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadGroupDependenciesAsync(overrideManager ?? _defaultManager);
        }

        /// <summary>
        /// Unloads all extra scenes.
        /// </summary>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadExtrasAsync(ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadExtrasAsync(overrideManager ?? _defaultManager);
        }
        
        /// <summary>
        /// Unloads a scene from the active group's dependencies.
        /// </summary>
        /// <param name="scenePath">The scene to unload.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadDependencyAsync(string scenePath, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadDependencyAsync(scenePath, overrideManager ?? _defaultManager);
        }
        
        /// <summary>
        /// Unloads a scene from the active group's dependencies.
        /// </summary>
        /// <param name="scene">The scene to unload.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadDependencyAsync(SceneReference scene, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadDependencyAsync(scene.Path, overrideManager ?? _defaultManager);
        }
        
        /// <summary>
        /// Unloads a loaded extra scene.
        /// </summary>
        /// <param name="scenePath">The scene to unload.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadExtraAsync(string scenePath, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadExtraAsync(scenePath, overrideManager ?? _defaultManager);
        }

        /// <summary>
        /// Unloads a loaded extra scene.
        /// </summary>
        /// <param name="scene">The scene to unload.</param>
        /// <param name="overrideManager">An override scene manager to specify how each scene should be unloaded.</param>
        public static async Task UnloadExtraAsync(SceneReference scene, ISceneManager overrideManager = null)
        {
            Verify();

            await _controller.UnloadExtraAsync(scene.Path, overrideManager ?? _defaultManager);
        }
    }
}