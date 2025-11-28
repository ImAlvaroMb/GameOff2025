using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TutorialScreen : MonoBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    public GameObject TutCamera;
    public CanvasGroup Canvas;

    public float scaleUpDuration = 0.15f;    
    public float scaleDownDuration = 0.05f;  
    public float maxScale = 1.2f;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(PopInEffect());
        if(TutCamera != null) TutCamera.SetActive(true);
        OnActivate.Invoke();
        PauseManager.Instance.SetIsShowingTutorialScreen(true);
    }

    private void OnDisable()
    {
        PauseManager.Instance.SetIsShowingTutorialScreen(false);
        if(TutCamera != null) TutCamera.SetActive(false);
        OnDeactivate.Invoke();
    }

    private IEnumerator PopInEffect()
    {
        float timer = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * maxScale;

        while (timer < scaleUpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / scaleUpDuration;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
        timer = 0f;
        startScale = endScale;
        endScale = Vector3.one;

        while (timer < scaleDownDuration)
        {
            timer += Time.deltaTime;
            float t2 = timer / scaleDownDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t2);
            yield return null; // Wait until next frame
        }

        // Ensure it lands on the final scale exactly
        transform.localScale = Vector3.one;
    }
}
