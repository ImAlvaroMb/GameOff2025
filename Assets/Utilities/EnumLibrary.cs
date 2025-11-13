using UnityEngine;
namespace Enums 
{
    public enum TimerDirection
    {
        INCREASE,
        DECREASE
    }

    public enum NPCActions
    {
        NONE,
        PATROL,
        DO_OBJECT_INTERACTION,
        TALK_TO_NPC,
        WAVE
    }

    public enum InteractableType
    {
        ALL,
        MUSIC,
        B
    }

    public enum TalkType
    {
        TALKER,
        LISTENER,
        NONE
    }

    public enum ChooseActionUIType
    {
        TALK_TO,
        WAVE_TO,
        CAMERA_FOLLOW,
        SELECT
    }
    
}
