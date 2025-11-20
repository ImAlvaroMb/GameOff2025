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
        if(value)
        {
            // start animating effect
        } else
        {
            // stop animating effect
        }
    }
}
