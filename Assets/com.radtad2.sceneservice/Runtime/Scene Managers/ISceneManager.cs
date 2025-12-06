namespace SceneService
{
    public interface ISceneManager
    {
        /// <summary>
        /// Starts loading a scene asynchronously and returns a handle to observe its progress.
        /// </summary>
        /// <param name="scenePath">The scene path to load.</param>
        /// <returns>A handle to observe its progress.</returns>
        public ISceneLoadOperation LoadSceneAsync(string scenePath);
        
        /// <summary>
        /// Starts unloading a scene asynchronously and returns a handle to observe its progress.
        /// </summary>
        /// <param name="scenePath">The scene path to unload.</param>
        /// <returns>A handle to observe its progress.</returns>
        public ISceneLoadOperation UnloadSceneAsync(string scenePath);
        
        /// <summary>
        /// Sets the loaded scene at the path active.
        /// </summary>
        /// <param name="scenePath">The path to set active</param>
        public void SetActiveScene(string scenePath);
    }
}