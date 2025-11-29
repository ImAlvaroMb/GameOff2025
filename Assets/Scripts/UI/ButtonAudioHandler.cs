using UnityEngine;
using UnityEngine.EventSystems;
using Enums;

public class ButtonAudioHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot(SoundName.UIHOVER);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot(SoundName.UICLICK);
    }
}
