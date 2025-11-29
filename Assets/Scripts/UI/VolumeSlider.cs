using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    private void OnEnable()
    {
        volumeSlider.value = AudioManager.Instance.GetVolume();
    }

}
