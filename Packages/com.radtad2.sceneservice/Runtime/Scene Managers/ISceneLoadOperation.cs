namespace SceneService
{
    public interface ISceneLoadOperation
    {
        /// <summary>
        /// Tells if the scene load is completed.
        /// </summary>
        public bool IsDone();
        
        /// <summary>
        /// Tells the progress for a scene load.
        /// </summary>
        public float Progress();
    }
}