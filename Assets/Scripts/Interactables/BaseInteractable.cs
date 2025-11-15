using Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    public List<InteractableType> InteractableType = new List<InteractableType>();
    public UnityEvent OnInteracted;

    private SpriteRenderer _sprite;
    private bool _canInteract = false; // this can interact is for the user direct interaction (mouse, not from a controlled NPC)

    private Vector3 _originalScale;
    [SerializeField] private float highlightScaleFactor = 1.3f;
    [SerializeField] private float animationDuration = 0.1f;

    private ITimer _currentTimer;

    protected virtual void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
    }
    public virtual void Interact(NPCController interactingNPC)
    {
        OnInteracted?.Invoke();
    }
    public virtual void Highlight()
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

    public virtual void Dehighlight()
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
        return _canInteract;
    }

    private void OnDestroy()
    {
        _currentTimer?.StopTimer();
    }

}
