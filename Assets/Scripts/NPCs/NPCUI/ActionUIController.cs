using System.Collections.Generic;
using UnityEngine;
using Enums;
public class ActionUIController : MonoBehaviour
{
    public List<UIActionButton> ActionsButtons = new List<UIActionButton>();

    public void SetActionsButtonsVisibility(bool isCurrentlyControlledNPC, NPCController thisNPC)
    {
        if(isCurrentlyControlledNPC) //its the currently controlled npc
        {
            foreach(UIActionButton action in ActionsButtons)
            {
                if(action.ChooseActionUIType == ChooseActionUIType.CAMERA_FOLLOW)
                {
                    action.gameObject.SetActive(true);
                } else
                {
                    action.gameObject.SetActive(false);
                }
            }
        } else
        {
            foreach(UIActionButton action in ActionsButtons)
            {
                if(thisNPC.IsFullyControlled) //is under controll but its not the one currently selected
                {
                    action.gameObject.SetActive(true);
                } else
                {
                    if(MouseInputController.Instance.CurrentlySelectedNPC == null)
                    {
                        if(action.ChooseActionUIType == ChooseActionUIType.CAMERA_FOLLOW)
                        {
                            action.gameObject.SetActive(true);
                        } else
                        {
                            action.gameObject.SetActive(false);
                        }
                    } else
                    {
                        if(action.ChooseActionUIType == ChooseActionUIType.SELECT)
                        {
                            action.gameObject.SetActive(false);
                        } else
                        {
                            action.gameObject.SetActive(true);
                        }
                    }
                }
                
            }
        }
    }
}
