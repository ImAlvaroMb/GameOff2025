using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Enums;
public class TransitionsScreen : MonoBehaviour
{
    public TextMeshProUGUI LoreTextRef;
    public string[] LoreText;
    private int _currentTextIndex = 0;
    public float TypingSpeed = 0.05f;

    public float BlinkInterval;

    public CanvasGroup ContinueText;
    private bool _isTyping = false;
    private bool _isBlinking = false;

    public UnityEvent OnLoreFinished;


    private void Start()
    {
        LoreTextRef.text = "";
        ContinueText.alpha = 0f;
        Invoke("StartTyping", 1f);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_isBlinking)
            {
                if(_currentTextIndex < LoreText.Length - 1)
                {
                    AudioManager.Instance.PlayOneShot(SoundName.LORENEXT);
                    StopBlinking();
                    _currentTextIndex++;
                    StartTyping();
                } else if(_currentTextIndex == LoreText.Length -1)
                {
                    StopBlinking();
                    OnLoreFinished.Invoke();
                }
            }
        }
    }

    private void StartTyping()
    {
        ContinueText.alpha = 0f;
        StartCoroutine(TypeMessage());
    }

    private IEnumerator TypeMessage()
    {
        AudioManager.Instance.PlayWithIdentifier(SoundName.LORESCROLL, Vector3.zero, "Scroll");
        ContinueText.alpha = 0f;
        _isTyping = true;
        LoreTextRef.text = "";
        for (int i = 0; i < LoreText[_currentTextIndex].Length; i++)
        {
            LoreTextRef.text += LoreText[_currentTextIndex][i];
            yield return new WaitForSeconds(TypingSpeed);
        }
        StartBlinking();
        _isTyping = false;

    }

    private void StartBlinking()
    {
        AudioManager.Instance.StopSoundByIdentifier("Scroll");
        ContinueText.gameObject.SetActive(true);
        _isBlinking = true;
        StartCoroutine(BlinkCoroutine());
    }

    private void StopBlinking()
    {
        if(_isBlinking)
        {
            StopCoroutine(BlinkCoroutine());
            _isBlinking = false;
        }
        ContinueText.gameObject.SetActive(false);
    }

    private IEnumerator BlinkCoroutine()
    {
        while (_isBlinking)
        {
            float timer = 0f;

            // --- Phase 1: Fade Out (1.0 -> 0.0) ---
            while (timer < BlinkInterval)
            {
                timer += Time.deltaTime;
                // Lerp from the current alpha to 0.0 over the duration
                ContinueText.alpha = Mathf.Lerp(1f, 0f, timer / BlinkInterval);
                yield return null;
            }
            ContinueText.alpha = 0f; // Ensure it hits the minimum value

            timer = 0f;

            // --- Phase 2: Fade In (0.0 -> 1.0) ---
            while (timer < BlinkInterval)
            {
                timer += Time.deltaTime;
                // Lerp from the current alpha to 1.0 over the duration
                ContinueText.alpha = Mathf.Lerp(0f, 1f, timer / BlinkInterval);
                yield return null;
            }
            ContinueText.alpha = 1f; // Ensure it hits the maximum value
        }
    }



}
