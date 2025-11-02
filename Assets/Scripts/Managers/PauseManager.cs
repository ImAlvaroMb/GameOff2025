using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class PauseManager : AbstractSingleton<PauseManager>
{
    private List<IPausable> _pausableElements = new List<IPausable>();

    public GameObject pauseTestingScreem;

    private bool _isGamePaused = false;

    public void HandlePausePressed(InputAction.CallbackContext ctx)
    {
        if (_isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (_isGamePaused) return;

        _isGamePaused = true;
        pauseTestingScreem.SetActive(true);

        foreach (IPausable element in _pausableElements)
        {
            element?.OnPause();
        }
    }

    private void ResumeGame()
    {
        if (!_isGamePaused) return;

        _isGamePaused = false;
        pauseTestingScreem.SetActive(false);

        foreach (IPausable element in _pausableElements)
        {
            element?.OnResume();
        }
    }

    public void RegistedPausable(IPausable pausable)
    {
        if (pausable == null)
        {
            Debug.LogError("Tried to register null pausable"); 
            return;
        }
        if (!_pausableElements.Contains(pausable)) _pausableElements.Add(pausable);
    }

    public void UnregistedPausable(IPausable pausable)
    {
        if(pausable == null)
        {
            Debug.LogError("Tried to unregisted a null pausable");
            return;
        }

        _pausableElements.Remove(pausable);
    }
}