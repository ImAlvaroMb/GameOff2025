using UnityEngine;
using Enums;
using UnityEngine.UI;

public class UIActionButton : MonoBehaviour
{
    public ChooseActionUIType ChooseActionUIType;

    private Button _button;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }
    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
    }

    private void OnButtonClicked()
    {
        MouseInputController.Instance.HandleNPCActionUIClick(ChooseActionUIType);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
}
