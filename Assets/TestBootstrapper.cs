using System;
using SceneService;
using UnityEngine;

public class TestBootstrapper : MonoBehaviour
{
    private ISceneController _controller;
    
    private async void Start()
    {
        try
        {
            _controller                 = SceneService.SceneService.BuildSceneController();
            _controller.OnLoadStart    += sceneName => Debug.Log($"Loading {sceneName}");
            _controller.OnProgress     += info      => Debug.Log($"Loading progress: {info.Progress}");
            _controller.OnLoadComplete += info      => Debug.Log($"Loaded {info.Group} with success: {info.Success}");
            
            await _controller.LoadGroupAsync("Level 1");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}