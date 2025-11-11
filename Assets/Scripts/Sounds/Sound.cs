using UnityEngine;
[System.Serializable]
public class Sound 
{
    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume;

    [HideInInspector]
    public AudioSource Source;

    public bool Loop;

    [Range(0f, 3f)]
    public float Pitch;

    [Range(1f, 500f)]
    public float MaxDistance;

    [Range(0f, 1f)]
    public float SpatialBlend;
}
