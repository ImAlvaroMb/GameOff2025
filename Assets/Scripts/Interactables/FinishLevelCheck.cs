using UnityEngine;
using UnityEngine.Events;

public class FinishLevelCheck : MonoBehaviour
{
    public NPCController NPCToFinishLevel;
    public UnityEvent OnWin;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        NPCController controller =  collision.gameObject.GetComponentInParent<NPCController>();
        if(controller != null)
        {
            if (controller == NPCToFinishLevel && controller.GetIsCarryingFrog())
            {
                OnWin.Invoke();
            }
        }
        
    }
}
