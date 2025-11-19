using System;
using UnityEngine;
using Enums;

[Serializable]  
public class CustomAnimation
{
    public NPCAnimation animationType;
    public Sprite[] sprites;
    [Range(0.1f, 10f)]
    public float framesPerSecond;

}
