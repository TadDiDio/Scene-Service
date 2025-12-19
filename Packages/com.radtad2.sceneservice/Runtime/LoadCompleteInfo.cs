namespace SceneService
{
    /// <summary>
    /// Gives info about a scene group load completion.
    /// </summary>
    public struct LoadCompleteInfo
    {
        /// <summary>
        /// The name of the group that was loaded.
        /// </summary>
        public readonly string Group;
        
        /// <summary>
        /// Tells if the load was successful or not.
        /// </summary>
        /// <remarks>Note that an unsuccessful load may be unrecoverable due to partial scene load status.
        /// The best course of action is likely to restart the app by loading the bootstrapper in single mode again.</remarks>
        public readonly bool Success;

        public LoadCompleteInfo(string group, bool success)
        {
            Group = group;
            Success = success;
        }
    }
}