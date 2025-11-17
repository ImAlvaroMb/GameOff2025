using UnityEngine;
using UnityEngine.Events;

public class FinishLevelCheck : MonoBehaviour
{
    public NPCController NPCToFinishLevel;
    public UnityEvent OnWin;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<NPCController>() == NPCToFinishLevel && collision.gameObject.GetComponent<NPCController>().GetIsCarryingFrog()) 
        {
            OnWin.Invoke();
        }
    }
}
