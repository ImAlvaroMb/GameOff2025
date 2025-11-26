using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

public class LevelController : AbstractSingleton<LevelController>
{
    public TextMeshProUGUI TimerText;
    public Image TimerFillImage;
    public Image TimerImage;
    [SerializeField] private float timeToLose;
    private ITimer _currentTimer = null;
    private List<SuspiciousNPC> _suspiciousNPCs = new List<SuspiciousNPC>();
    public UnityEvent OnLose;

    protected override void Start()
    {
        base.Start();
        FindAllSuspuciousNPC();
        //AlertSystemController.Instance.SendAlert("CARRY THE FROG TO THE EXIT WITH A WORTHY NPC", 2.5f);
    }

    [ContextMenu("Find all NPC")]
    public void FindAllSuspuciousNPC()
    {
        _suspiciousNPCs.Clear();
        _suspiciousNPCs = FindObjectsByType<SuspiciousNPC>(FindObjectsSortMode.None).ToList();
    }

    public void FrogIsMovgin(NPCController controller)
    {
        NotifyAllSuspiciousNPCOfNewTarget(controller);
        TimerFillImage.gameObject.SetActive(true);
        TimerImage.gameObject.SetActive(true);
        _currentTimer = TimerSystem.Instance.CreateTimer(timeToLose, onTimerDecreaseComplete: () =>
        {
            TimerText.text = "0";
            TimerFillImage.fillAmount = 0f;
            OnLose.Invoke();
        }, onTimerDecreaseUpdate: (progress) =>
        {
            float value = progress / timeToLose;
            TimerText.text = progress.ToString();
            TimerFillImage.fillAmount = value;
        });
    }

    public void FrogIsBackToPlace(NPCController controller)
    {
        NotifyAllSuspiciousNPCOfNewTarget(null);
        if(_currentTimer != null)
        {
            _currentTimer.StopTimer();
            TimerText.text = string.Empty;
            TimerFillImage.fillAmount = 0;
            TimerFillImage.gameObject.SetActive(false);
            TimerImage.gameObject.SetActive(false);
        }
    }

    public void NotifyAllSuspiciousNPCOfNewTarget(NPCController controller)
    {
        foreach(SuspiciousNPC npc in _suspiciousNPCs)
        {
            npc.SetCurrentTargetToCheck(controller);
        }
    }

}
