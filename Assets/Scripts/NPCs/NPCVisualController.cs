using Enums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class NPCVisualController : MonoBehaviour
{
    [SerializeField] private Image influenceMeterImg;
    [SerializeField] private ActionUIController hoverButtons;
    [SerializeField] private NPCStateUIController npcStateUIController;
    private const float DIRECTION_THRESHOLD = 0.02f;

    [Header("Sprite Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    public CustomAnimation[] animations;
    private Dictionary<NPCAnimation, CustomAnimation> _animationMap = new Dictionary<NPCAnimation, CustomAnimation>();
    private NPCAnimation _currentAnimationType = NPCAnimation.DOWN_IDLE;
    private WalkDirection lastDirection = WalkDirection.DOWN;
    private float _frameTimer;
    private int _currentFrameIndex;

    [Header("SpeechBubble")]
    [SerializeField] private string[] phrases;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private float bubbleDuration;

    [Header("Inmunne")]
    [SerializeField] private GameObject hatObj;
    [SerializeField] private GameObject inmunneAreaObj;

    private NPCController _controller;

    private void Awake()
    {
        foreach (CustomAnimation animation in animations)
        {
            if(!_animationMap.ContainsKey(animation.animationType))
            {
                _animationMap.Add(animation.animationType, animation);
            }
        }    
    }

    private void Start()
    {
        _controller = GetComponent<NPCController>();
        UpdateInfluenceMeterImage(0f);
        hoverButtons?.gameObject.SetActive(false);
        PlayAnimation(NPCAnimation.DOWN_IDLE);

        if(_controller != null && _controller.IsInmunne)
        {
            hatObj.SetActive(true);
        } else
        {
            hatObj.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateSpriteAnimation();
    }
    #region Animations

    public void PlayAnimation(NPCAnimation newAnimationType)
    {
        if (_currentAnimationType == newAnimationType || !_animationMap.ContainsKey(newAnimationType))
        {
            return;
        }

        _currentAnimationType = newAnimationType;
        _currentFrameIndex = 0;
        _frameTimer = 0f;

        // Set the very first frame immediately
        CustomAnimation newAnim = _animationMap[newAnimationType];
        if (newAnim.sprites.Length > 0)
        {
            spriteRenderer.sprite = newAnim.sprites[0];
        }
    }

    private void UpdateSpriteAnimation()
    {
        if (!_animationMap.ContainsKey(_currentAnimationType))
        {
            return;
        }

        CustomAnimation currentAnim = _animationMap[_currentAnimationType];

        if (currentAnim.sprites == null || currentAnim.sprites.Length == 0) return;

        float timePerFrame = 1f / currentAnim.framesPerSecond;
        _frameTimer += Time.deltaTime;

        if (_frameTimer >= timePerFrame)
        {
            _frameTimer -= timePerFrame; 

            // Advance frame index, wrapping around to the start
            _currentFrameIndex = (_currentFrameIndex + 1) % currentAnim.sprites.Length;

            spriteRenderer.sprite = currentAnim.sprites[_currentFrameIndex];
        }
    }
    private NPCAnimation GetWalkAnimation(WalkDirection direction)
    {
        return (NPCAnimation)Enum.Parse(typeof(NPCAnimation), direction.ToString() + "_WALK");
    }

    private NPCAnimation GetIdleAnimation(WalkDirection direction)
    {
        return (NPCAnimation)Enum.Parse(typeof(NPCAnimation), direction.ToString() + "_IDLE");
    }

    public void DetermineCardinalDirection(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        WalkDirection newDirection;

        if (absX < DIRECTION_THRESHOLD && absY < DIRECTION_THRESHOLD)
        {
            NoMovement();
            return;
        }

        if (absX > absY)
        {
            newDirection = (direction.x > DIRECTION_THRESHOLD) ? WalkDirection.RIGHT : WalkDirection.LEFT;
        }
        else 
        {
            newDirection = (direction.y > DIRECTION_THRESHOLD) ? WalkDirection.UP : WalkDirection.DOWN;
        }

        lastDirection = newDirection;
        NPCAnimation newAnim = GetWalkAnimation(newDirection);
        PlayAnimation(newAnim);
    }

    public void NoMovement()
    {
        PlayAnimation(GetIdleAnimation(lastDirection));
    }

    #endregion


    public void OnHovered(bool isCurrentlySelectedNPC)
    {
        hoverButtons.gameObject.SetActive(true);
        hoverButtons.SetActionsButtonsVisibility(isCurrentlySelectedNPC, _controller);
    }

    public void OnStopHovering()
    {
        hoverButtons.gameObject.SetActive(false);
    }

    public void OnAction(NPCActions action)
    {
        npcStateUIController.ChangeCurrentActionBeingDone(action);
    }

    public void OnControlledChanged(bool value)
    {
        npcStateUIController.ChangeBeingControlled(value);
    }

    public void UpdateInfluenceMeterImage(float fillAmount)
    {
        if(influenceMeterImg != null)
            influenceMeterImg.fillAmount = fillAmount;
    }

    public void ActivateSpeechBubble(Action onSpeechBubbleFinish)
    {
        speechBubble.SetActive(true);
        bubbleText.text = GetRandomPhrase();
        TimerSystem.Instance.CreateTimer(bubbleDuration, onTimerDecreaseComplete: () => 
        {
            speechBubble.SetActive(false); 
            onSpeechBubbleFinish?.Invoke();
        });
    }

    public string GetRandomPhrase()
    {
        int index = UnityEngine.Random.Range(0, phrases.Length);
        return phrases[index];
    }

}
