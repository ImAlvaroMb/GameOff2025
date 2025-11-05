using UnityEngine;
using Utilities;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sprite;
    private bool canInteract = true;

    private Vector3 _originalScale;
    [SerializeField] private float highlightScaleFactor = 1.3f;
    [SerializeField] private float animationDuration = 0.1f;

    private ITimer _currentTimer;

    protected virtual void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
    }
    public void Interact()
    {
        Debug.Log($"Clicked on: {gameObject.name} interactable object");
    }
    public void Highlight()
    {
        _currentTimer?.StopTimer();
        Vector3 targetScale = _originalScale * highlightScaleFactor;
        _currentTimer = TimerSystem.Instance.CreateTimer(animationDuration, onTimerDecreaseComplete: () =>
        {
            transform.localScale = targetScale;
            _currentTimer = null;
        }, onTimerDecreaseUpdate: (progress) =>
        {
            float timeElapsed = animationDuration - progress;
            float t = timeElapsed / animationDuration;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
        });
    }

    public void Dehighlight()
    {
        _currentTimer?.StopTimer();
        _currentTimer = TimerSystem.Instance.CreateTimer(animationDuration, onTimerDecreaseComplete: () =>
        {
            transform.localScale = _originalScale;
            _currentTimer = null;
        }, onTimerDecreaseUpdate: (progress) =>
        {
            float timeElapsed = animationDuration - progress;
            float t = timeElapsed / animationDuration;
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, t);
        });
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    private void OnDestroy()
    {
        _currentTimer?.StopTimer();
    }

}
