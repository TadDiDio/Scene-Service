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
            _controller = SceneService.SceneService.BuildSceneController();
        
            await _controller.LoadGroupAsync("Level 1");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}