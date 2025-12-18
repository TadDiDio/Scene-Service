namespace SceneService
{
    /// <summary>
    /// Determines which loaded scenes to reload when the target scene group contains them.
    /// </summary>
    public enum ReloadPolicy
    {
        /// <summary>
        /// Don't reload anything - keep all existing loaded scenes that the next scene group needs.
        /// </summary>
        None,
        
        /// <summary>
        /// Only the currently active scene will be reloaded if it is needed by the target scene group.
        /// </summary>
        Active,
        
        /// <summary>
        /// Only the currently active dependencies will be reloaded if they are needed by the target scene group.
        /// </summary>
        Dependencies,
        
        /// <summary>
        /// Only the currently active extras will be reloaded if they are needed by the target scene group.
        /// </summary>
        Extras,
        
        /// <summary>
        /// Only the active scene and currently active dependencies will be reloaded if they are needed by the target scene group.
        /// </summary>
        ActiveAndDependencies,
        
        /// <summary>
        /// Only the active scene and currently active extras will be reloaded if they are needed by the target scene group.
        /// </summary>
        ActiveAndExtras,
        
        /// <summary>
        /// Only the currently active dependencies and extras will be reloaded if they are needed by the target scene group.
        /// </summary>
        DependenciesAndExtras,
        
        /// <summary>
        /// Reloads all currently active scenes that are needed by the target scene group.
        /// </summary>
        All,
    }
}