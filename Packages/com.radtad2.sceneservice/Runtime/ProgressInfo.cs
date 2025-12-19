namespace SceneService
{
    /// <summary>
    /// Gives info about scene load progress.
    /// </summary>
    public struct ProgressInfo
    {
        /// <summary>
        /// The group being loaded.
        /// </summary>
        public readonly string Group;

        /// <summary>
        /// The progress of the load [0, 1].
        /// </summary>
        public readonly float Progress;

        public ProgressInfo(string group, float progress)
        {
            Group = group;
            Progress = progress;
        }
    }
}