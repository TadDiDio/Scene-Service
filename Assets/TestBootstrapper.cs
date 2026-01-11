using System;
using System.Threading.Tasks;
using SceneService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestBootstrapper : MonoBehaviour
{
    private ISceneController _controller;
    
    private async void Start()
    {
        try
        {
            _controller = Scenes.BuildSceneController();

            _controller.OnExternalControlBegin += () => print("Begin external control");
            _controller.OnExternalControlEnd   +=  g => print("End external control with group: " + g.GroupName);
            
            await _controller.LoadGroupAsync("Level 1");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _ = HandleStartExternalAsync();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            _ = HandleEndExternalAsync();
        }
    }

    private async Task HandleStartExternalAsync()
    {
        await _controller.BeginExternalControl();
        await SceneManager.LoadSceneAsync("Level 2", LoadSceneMode.Additive);
    }

    private async Task HandleEndExternalAsync()
    {
        await _controller.EndExternalControl("Level 1");
    }
}