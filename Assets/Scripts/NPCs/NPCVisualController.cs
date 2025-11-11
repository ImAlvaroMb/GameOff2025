using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class NPCVisualController : MonoBehaviour
{
    [SerializeField] private Image influenceMeterImg;
    [SerializeField] private Animator animator;

    private const float DIRECTION_THRESHOLD = 0.02f;

    [Header("SpeechBubble")]
    [SerializeField] private string[] phrases;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private float bubbleDuration;
    private void Start()
    {
        UpdateInfluenceMeterImage(0f);
    }

    public void UpdateInfluenceMeterImage(float fillAmount)
    {
        influenceMeterImg.fillAmount = fillAmount;
    }

    public void NoMovement()
    {
        animator.SetBool("WalkingRight", false);
        animator.SetBool("WalkingLeft", false);
        animator.SetBool("WalkingUp", false);
        animator.SetBool("WalkingDown", false); 
    }

    

    public void DetermineCardinalDirection(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        if(absX > absY)
        {
            if(direction.x > DIRECTION_THRESHOLD)
            {
                animator.SetBool("WalkingRight", true);
                animator.SetBool("WalkingLeft", false);
                animator.SetBool("WalkingUp", false);
                animator.SetBool("WalkingDown", false);
            } else if (direction.x < -DIRECTION_THRESHOLD)
            {
                animator.SetBool("WalkingLeft", true);
                animator.SetBool("WalkingRight", false);
                animator.SetBool("WalkingUp", false);
                animator.SetBool("WalkingDown", false);
            }
        } else if(absY > absX)
        {
            if(direction.y > DIRECTION_THRESHOLD)
            {
                animator.SetBool("WalkingUp", true);
                animator.SetBool("WalkingDown", false);
                animator.SetBool("WalkingRight", false);
                animator.SetBool("WalkingLeft", false);

            } else if(direction.y < -DIRECTION_THRESHOLD)
            {
                animator.SetBool("WalkingDown", true);
                animator.SetBool("WalkingUp", false);
                animator.SetBool("WalkingRight", false);
                animator.SetBool("WalkingLeft", false);
            }
        }
    }

    public void ActivateSpeechBubble()
    {
        speechBubble.SetActive(true);
        bubbleText.text = GetRandomPhrase();
        TimerSystem.Instance.CreateTimer(bubbleDuration, onTimerDecreaseComplete: () => speechBubble.SetActive(false));
    }

    public string GetRandomPhrase()
    {
        int index = Random.Range(0, phrases.Length);
        return phrases[index];
    }

}
