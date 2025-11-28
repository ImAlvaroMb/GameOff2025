using TMPro;
using UnityEngine;
using Utilities;
public class AlertSystemController : AbstractSingleton<AlertSystemController>
{
    public TextMeshProUGUI TextToUpdate;
    public GameObject NotificationBox;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;

    private ITimer _currentTimer = null;


    protected override void Start()
    {
        base.Start();
        NotificationBox.SetActive(false);
    }

    public void SendAlert(string text, float duration)
    {
        NotificationBox.SetActive(true);
        TextToUpdate.alpha = 0f;
        _currentTimer?.StopTimer();
        StartTimer(text, duration);
    }

    private void StartTimer(string text, float duration)
    {
        TextToUpdate.text = text;
        _currentTimer = TimerSystem.Instance.CreateTimer(fadeInDuration, Enums.TimerDirection.INCREASE, onTimerIncreaseComplete: () =>
        {
            _currentTimer = null;
            TextToUpdate.alpha = 1f;
            ShowTextAndFadeOut(duration);
        }, onTimerIncreaseUpdate: (progress) =>
        {
            float value = progress / fadeInDuration;
            TextToUpdate.alpha = progress;
        });
    }

    private void ShowTextAndFadeOut(float duration)
    {
        _currentTimer = TimerSystem.Instance.CreateTimer(duration, onTimerDecreaseComplete: () =>
        {
            _currentTimer = TimerSystem.Instance.CreateTimer(fadeOutDuration, onTimerDecreaseComplete: () =>
            {
                TextToUpdate.alpha = 0f;
                _currentTimer = null;
                NotificationBox.SetActive(false);
            }, onTimerDecreaseUpdate: (progress) =>
            {
                float value = progress / fadeOutDuration;
                TextToUpdate.alpha = progress;
            });
        });
    }



   
}
