namespace SceneService
{
    public interface ISceneManager
    {
        public ISceneLoadOperation LoadSceneAsync(string scenePath);
        
        public ISceneLoadOperation UnloadSceneAsync(string scenePath);
    }
}