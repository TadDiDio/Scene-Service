using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Tester : MonoBehaviour
{
    public SceneReference scene;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadSceneAsync(scene.Path, LoadSceneMode.Additive);
        }
    }
}
