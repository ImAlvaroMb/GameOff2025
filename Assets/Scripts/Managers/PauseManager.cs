using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

public class PauseManager : AbstractSingleton<PauseManager>
{
    private List<IPausable> _pausableElements = new List<IPausable>();

    public GameObject pauseTestingScreen;

    private bool _isGamePaused = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HandlePausePressed();
        }
    }

    public void HandlePausePressed()
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
        pauseTestingScreen.SetActive(true);

        foreach (IPausable element in _pausableElements)
        {
            element?.OnPause();
        }
    }

    private void ResumeGame()
    {
        if (!_isGamePaused) return;

        _isGamePaused = false;
        pauseTestingScreen.SetActive(false);

        foreach (IPausable element in _pausableElements)
        {
            element?.OnResume();
        }
    }

    public void RegisterPausable(IPausable pausable)
    {
        if (pausable == null)
        {
            Debug.LogError("Tried to register null pausable"); 
            return;
        }
        if (!_pausableElements.Contains(pausable)) _pausableElements.Add(pausable);
    }

    public void UnregisterPausable(IPausable pausable)
    {
        if(pausable == null)
        {
            Debug.LogError("Tried to unregisted a null pausable");
            return;
        }

        _pausableElements.Remove(pausable);
    }
}