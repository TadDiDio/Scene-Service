using SceneService;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneMapWindow : EditorWindow
{
    private SceneMap _settings;
    private SerializedObject _settingsSO;
    private string _assetPath;
    private Vector2 _scroll;
    
    
    [MenuItem("Tools/Scene Map Settings")]
    public static void Open()
    {
        var window = GetWindow<SceneMapWindow>("Scene Map Settings");
        window.minSize = new Vector2(420, 260);
        window.Show();
    }
    
    private void OnEnable()
    {
        var guid = EditorPrefs.GetString(SceneMap.EditorKey, "");
        
        if (!string.IsNullOrEmpty(guid))
        {
            _assetPath = AssetDatabase.GUIDToAssetPath(guid);
            _settings = SceneMap.Active;
            _settingsSO = new SerializedObject(_settings);

            if (_settings.LoadBootstrapperFirst)
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_settings.BootstrapScene.Path);
            }
        }
    }
    
    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Scene mapping", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawSettingsSelector();
        if (_settings) DrawSelectedSettings();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSettingsSelector()
    {
        EditorGUILayout.LabelField("Select Settings Asset", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        _settings = (SceneMap)EditorGUILayout.ObjectField("Settings Asset", _settings, typeof(SceneMap), false);
        
        if (_settings) _assetPath = AssetDatabase.GetAssetPath(_settings);

        if (EditorGUI.EndChangeCheck())
        {
            if (_settings)
            {
                _settingsSO = new SerializedObject(_settings);
                EditorPrefs.SetString(SceneMap.EditorKey, AssetDatabase.AssetPathToGUID(_assetPath));
            }
            else
            {
                _assetPath = null;
                _settingsSO = null;
                EditorPrefs.SetString(SceneMap.EditorKey, string.Empty);
            }
        }

        if (_settings && !PathIsInResources(_assetPath))
        {
            EditorGUILayout.HelpBox($"{nameof(SceneMap)} must be stored inside a Resources folder.\n\n" + "Move it into ANY_FOLDER/Resources/ to ensure it loads at runtime.", MessageType.Error);
        }

        EditorGUILayout.Space();
    }

    private void DrawSelectedSettings()
    {
        if (_settingsSO == null) return;

        var bootSceneUpdated = false;

        if (_settings.LoadBootstrapperFirst)
        {
            GUILayout.Label("<color=green>Bootstrap scene will load first</color>", new GUIStyle {richText = true});
            if (GUILayout.Button("Don't load Bootstrap scene first"))
            {
                EditorSceneManager.playModeStartScene = null;
                _settings.LoadBootstrapperFirst = false;
                bootSceneUpdated = true;
            }
        }
        else
        {
            GUILayout.Label("<color=red>Bootstrap scene will not load first</color>", new GUIStyle {richText = true});
            if (GUILayout.Button("Load Bootstrap scene first"))
            {
                _settings.LoadBootstrapperFirst = true;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_settings.BootstrapScene.Path);
                bootSceneUpdated = true;
            }
        }
        
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        _settingsSO.Update();

        var prop = _settingsSO.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            if (prop.name == "m_Script") continue;
            EditorGUILayout.PropertyField(prop, true);
            enterChildren = false;
        }

        if (_settingsSO.ApplyModifiedProperties() || bootSceneUpdated)
        {
            if (_settings.LoadBootstrapperFirst)
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_settings.BootstrapScene.Path);
            }
            EditorUtility.SetDirty(_settings);
        }

        var message = _settings.Validate();

        if (!string.IsNullOrEmpty(message))
        {
            EditorGUILayout.HelpBox(message, MessageType.Error);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private bool PathIsInResources(string path)
    {
        return !string.IsNullOrEmpty(path) && path.Replace("\\", "/").Contains("/Resources/");
    }
}
