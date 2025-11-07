using UnityEngine;
using UnityEngine.UI;

public class NPCVisualController : MonoBehaviour
{
    [SerializeField] private Image influenceMeterImg;


    private void Start()
    {
        UpdateInfluenceMeterImage(0f);
    }

    public void UpdateInfluenceMeterImage(float fillAmount)
    {
        influenceMeterImg.fillAmount = fillAmount;
    }
}
