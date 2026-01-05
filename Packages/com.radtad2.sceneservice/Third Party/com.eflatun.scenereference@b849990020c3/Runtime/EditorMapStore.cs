#if UNITY_EDITOR
using UnityEditor;

namespace Eflatun.SceneReference
{
    internal static class EditorMapStore
    {
        private static readonly string KeyPrefix = $"Eflatun_SceneReference_";

        private static readonly string SceneGuidToPathMapJson_Key = $"{KeyPrefix}SceneGuidToPathMapJson";

        /// <remarks>
        /// Null if missing.
        /// </remarks>
        public static string SceneGuidToPathMapJson
        {
            get => EditorPrefs.GetString(SceneGuidToPathMapJson_Key);
            set => EditorPrefs.SetString(SceneGuidToPathMapJson_Key, value);
        }
        
        private static readonly string SceneGuidToAddressMapJson_Key = $"{KeyPrefix}SceneGuidToAddressMapJson";

        /// <remarks>
        /// Null if missing.
        /// </remarks>
        public static string SceneGuidToAddressMapJson
        {
            get => EditorUserSettings.GetConfigValue(SceneGuidToAddressMapJson_Key);
            set => EditorUserSettings.SetConfigValue(SceneGuidToAddressMapJson_Key, value);
        }
    }
}
#endif // UNITY_EDITOR
