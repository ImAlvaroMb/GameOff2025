using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;
    private int _currentMaxLevel = 0;

    private const string LEVEL_KEY = "CuerrrentMaxLevel";
    private void OnEnable()
    {
        _currentMaxLevel = PlayerPrefs.GetInt(LEVEL_KEY, 0);   
        SetInteractableButtons();
    }

    private void SetInteractableButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if(i <= _currentMaxLevel)
            {
                levelButtons[i].interactable = true;
            } else
            {
                levelButtons[i].interactable = false;
            }
        }
    }
}
