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
        PLAYER,
        CONTROLLED,
        STUDENT,
        TEACHER
    }

    public enum TalkType
    {
        TALKER,
        LISTENER,
        NONE
    }

    public enum NPCWaveType
    {
        WAVER,
        WAVE_RECEIVER,
        NONE
    }

    public enum ChooseActionUIType
    {
        TALK_TO,
        WAVE_TO,
        CAMERA_FOLLOW,
        SELECT
    }

    public enum WalkDirection
    {
        DOWN,
        UP,
        LEFT,
        RIGHT
    }

    public enum NPCAnimation
    {
        DOWN_IDLE,
        DOWN_WALK,
        LEFT_IDLE,
        LEFT_WALK,
        RIGHT_IDLE,
        RIGHT_WALK,
        UP_IDLE,
        UP_WALK,
    }

    public enum SoundName
    {
        CONTROLON,
        CONTROLOFF,
        NPCDOOR,
        NPCFROG,
        NPCCANT,
        NPCRADIO,
        NPCSELECT,
        NPCTALK,
        NPCWALK,
        NPCWAVE,
        UICLICK,
        UIHOVER,
        LORENEXT,
        LORESCROLL
    }
    
}
