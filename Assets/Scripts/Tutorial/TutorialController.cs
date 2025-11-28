using UnityEngine;
using Utilities;

public class TutorialController : MonoBehaviour
{
    public GameObject StartTurorial;
    public float Delay;
    private void Start()
    {
        TimerSystem.Instance.CreateTimer(Delay, onTimerDecreaseComplete: () =>
        {
            StartTurorial.SetActive(true);
            PauseManager.Instance.ForcePauseGame();

        });    
    }

    public void OnTutorialFinished()
    {
        PauseManager.Instance.ForceUnpauseGame();
    }


}
