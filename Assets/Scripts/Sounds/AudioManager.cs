using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class AudioManager : AbstractSingleton<AudioManager>
{
    public Sound[] sounds;

    private Dictionary<string, GameObject> activeIdentificableAudios = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.maxDistance = s.MaxDistance;
            s.Source.spatialBlend = s.SpatialBlend;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public void PlayWithIdentifier(string name, Vector3 position, string id)
    {
        Sound s = Array.Find(sounds, Sound => Sound.Name == name);
        if (s == null) return;

        if(activeIdentificableAudios.ContainsKey(id))
        {
            return;
        } else
        {
            GameObject newObject = new GameObject();
            AudioSource newSource = newObject.AddComponent<AudioSource>();
            activeIdentificableAudios.Add(id, newObject);
            newSource.clip = s.Source.clip;
            newSource.transform.position = position;
            newSource.loop = s.Loop;
            newSource.spatialBlend = s.SpatialBlend;
            newSource.maxDistance = s.MaxDistance;
            newSource.volume = s.Volume;
            newSource.Play();
        }
    }

    public void StopSoundByIdentifier(string id)
    {
        if(activeIdentificableAudios.ContainsKey(id))
        {
            Destroy(activeIdentificableAudios[id]);
            activeIdentificableAudios.Remove(id);
        }
    }

    public bool GetIsPlayingIdentifeir(String id)
    {
        if (activeIdentificableAudios.ContainsKey(id))
        {
            return activeIdentificableAudios[id].GetComponent<AudioSource>().isPlaying;
        }

        return false;
    }

    public void StopAllAudios()
    {
        foreach (var pair in activeIdentificableAudios)
        {
            GameObject audioObject = pair.Value;
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public void ReestartAudios()
    {
        foreach (var pair in activeIdentificableAudios)
        {
            GameObject audioObject = pair.Value;
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
        }
    }

}
