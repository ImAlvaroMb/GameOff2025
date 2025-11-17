using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class LevelController : AbstractSingleton<LevelController>
{
    public TextMeshProUGUI TimerText;
    public Image TimerFillImage;
    public Image TimerImage;
    [SerializeField] private float timeToLose;
    private ITimer _currentTimer = null;

    protected override void Start()
    {
        base.Start();

        AlertSystemController.Instance.SendAlert("CARRY THE FROG TO THE EXIT WITH A WORTHY NPC", 2.5f);
    }

    public void FrogIsMovgin()
    {
        TimerFillImage.gameObject.SetActive(true);
        TimerImage.gameObject.SetActive(true);
        _currentTimer = TimerSystem.Instance.CreateTimer(timeToLose, onTimerDecreaseComplete: () =>
        {
            TimerText.text = "0";
            TimerFillImage.fillAmount = 0f;
            // call lose screen
        }, onTimerDecreaseUpdate: (progress) =>
        {
            float value = progress / timeToLose;
            TimerText.text = progress.ToString();
            TimerFillImage.fillAmount = value;
        });
    }

    public void FrogIsBackToPlace()
    {
        if(_currentTimer != null)
        {
            _currentTimer.StopTimer();
            TimerText.text = string.Empty;
            TimerFillImage.fillAmount = 0;
            TimerFillImage.gameObject.SetActive(false);
            TimerImage.gameObject.SetActive(false);
        }
    }

}
