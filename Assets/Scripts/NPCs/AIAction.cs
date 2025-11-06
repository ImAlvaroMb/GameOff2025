using UnityEngine;
using Enums;
[System.Serializable]
public class AIAction 
{
    public NPCActions Action;
    [Range(0f, 100f)]
    public float Probability = 0f;
}
