using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;

public class NPCStateUIController : MonoBehaviour
{
    [Header("Action UI")]
    public StateUIImage[] ActionImages;
    public Image ActionImage;
    public GameObject ActionImageContainer;
    private Dictionary<NPCActions, StateUIImage> _actionsImagesMap = new Dictionary<NPCActions, StateUIImage>();

    [Header("Controlled UI")]
    public GameObject ControlledImageContainer;
    public Image ControlledImage;
    [SerializeField] private float animationAmplitude = 0.1f;
    [SerializeField] private float animationFrequency = 6f;

    private bool _isControlled = false;
    private Vector2 _originalControlledImagePosition;

    private void Awake()
    {
        foreach (StateUIImage field in ActionImages)
        {
            if(!_actionsImagesMap.ContainsKey(field.Action))
            {
                _actionsImagesMap.Add(field.Action, field);
            }
        }
    }

    private void Start()
    {
        ChangeCurrentActionBeingDone(NPCActions.NONE);
        ChangeBeingControlled(false);
    }

    private void Update()
    {
        if(_isControlled)
        {
            float yOffset = Mathf.Sin(Time.time * animationFrequency) * animationAmplitude;

            ControlledImage.transform.position = new Vector2(
                ControlledImageContainer.transform.position.x,
                ControlledImageContainer.transform.position.y + yOffset
                );
        }
    }

    public void ChangeCurrentActionBeingDone(NPCActions action)
    {
        if(_actionsImagesMap.ContainsKey(action))
        {
            if(action == NPCActions.NONE || action == NPCActions.PATROL)
            {
                ActionImageContainer.SetActive(false);
                ActionImage.sprite = null;
            } else
            {
                ActionImageContainer.SetActive(true);
                ActionImage.sprite = _actionsImagesMap[action].ImageRef;
            }
        }
    }

    public void ChangeBeingControlled(bool value)
    {
        ControlledImageContainer.SetActive(value);
        _isControlled = value;
        if(value)
        {
            _originalControlledImagePosition = ControlledImage.transform.position;
        }
    }
}
