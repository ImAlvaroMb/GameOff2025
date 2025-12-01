using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
public class AudioManager : AbstractSingleton<AudioManager>, IPausable 
{
    public Sound[] sounds;

    private List<GameObject> activeAudios = new List<GameObject>();
    private Dictionary<string, GameObject> activeIdentificableAudios = new Dictionary<string, GameObject>();

    private bool _paused = false;
    private float _volume = 0.5f;
    private const string VOLUME_KEY = "MasterVolume";
    public UnityEvent PlayOnStart;
    public SoundName songToPlay;
    private GameObject currentSongObj;
    private float songProgressOnPause = 0;
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
        PlayOnStart.Invoke();
        _volume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.5f);
        UpdateAllActiveSoundsVolume(_volume);

        PlayWithIdentifier(songToPlay, Vector3.zero, songToPlay.ToString());
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {  

        } else
        {
            if(currentSongObj == null) 
            {
                PlayWithIdentifier(songToPlay, Vector3.zero, songToPlay.ToString());
            }
        }
    }

    public void PlayGameMusic()
    {
        PlayWithIdentifier(SoundName.GAMEMUSIC, Vector3.zero, SoundName.GAMEMUSIC.ToString());
    }

    public void PlayLoreMusic()
    {
        PlayWithIdentifier(SoundName.LOREMUSIC, Vector3.zero, SoundName.LOREMUSIC.ToString());
    }

    public void PlayMenuMusic()
    {
        PlayWithIdentifier(SoundName.MENUMUSIC, Vector3.zero, SoundName.MENUMUSIC.ToString());
    }


    private void FixedUpdate()
    {
        if(!_paused)
        {
            for (int i = 0; i < activeAudios.Count - 1; i++)
            {
                AudioSource source = activeAudios[i].GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    GameObject obj = activeAudios[i];
                    if(activeIdentificableAudios.ContainsValue(obj))
                    {
                        string key = activeIdentificableAudios.FirstOrDefault(pair => pair.Value == obj).Key;
                        activeIdentificableAudios.Remove(key);
                    }
                    activeAudios.RemoveAt(i);
                    Destroy(obj);
                } else
                {
                    GameObject obj = activeAudios[i];
                    if(obj == currentSongObj)
                    {
                        songProgressOnPause = currentSongObj.GetComponent<AudioSource>().time;
                    }
                }
            }
        }

        
    }

    public void UpdateAllActiveSoundsVolume(float value)
    {
        _volume = value;
        foreach (GameObject audio in activeAudios)
        {
            audio.GetComponent<AudioSource>().volume = _volume;
        }

        foreach (Sound s in sounds)
        {
            s.Volume = _volume;
        }

        PlayerPrefs.SetFloat(VOLUME_KEY, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return _volume;
    }

    public void PlayOneShot(SoundName sound)
    {
        string name = sound.ToString();
        Sound s = Array.Find(sounds, Sound => Sound.Name == name);
        if (s == null) return;
        GameObject newObject = new GameObject();
        AudioSource newSource = newObject.AddComponent<AudioSource>();
        newSource.clip = s.Source.clip;
        newSource.loop = s.Loop;
        newSource.spatialBlend = s.SpatialBlend;
        newSource.maxDistance = s.MaxDistance;
        newSource.volume = s.Volume;
        newSource.Play();
        activeAudios.Add(newObject);
    }

    public void PlayOneShotWithName(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.Name == name);
        if (s == null) return;
        GameObject newObject = new GameObject();
        AudioSource newSource = newObject.AddComponent<AudioSource>();
        newSource.clip = s.Source.clip;
        newSource.loop = s.Loop;
        newSource.spatialBlend = s.SpatialBlend;
        newSource.maxDistance = s.MaxDistance;
        newSource.volume = s.Volume;
        newSource.Play();
        activeAudios.Add(newObject);
    }

    public void PlayWithIdentifier(SoundName sound, Vector3 position, string id)
    {
        string name = sound.ToString();
        Sound s = Array.Find(sounds, Sound => Sound.Name == name);
        if (s == null) return;

        if(activeIdentificableAudios.ContainsKey(id))
        {
            return;
        } else
        {
            GameObject newObject = new GameObject();
            AudioSource newSource = newObject.AddComponent<AudioSource>();
            newSource.clip = s.Source.clip;
            newSource.transform.position = position;
            newSource.loop = s.Loop;
            newSource.spatialBlend = s.SpatialBlend;
            newSource.maxDistance = s.MaxDistance;
            newSource.volume = s.Volume;
            newSource.Play();
            activeIdentificableAudios.Add(id, newObject);
            activeAudios.Add(newObject);

            if (name == songToPlay.ToString())
            {
                currentSongObj = newObject;
                newSource.time = songProgressOnPause;
            }
        }

       
    }

    public void StopSoundByIdentifier(string id)
    {
        if(activeIdentificableAudios.ContainsKey(id))
        {

            activeAudios.Remove(activeIdentificableAudios[id]);
            Destroy(activeIdentificableAudios[id]);
            activeIdentificableAudios.Remove(id);
        }
    }

    public bool GetIsPlayingIdentifeir(string id)
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

    public void OnPause()
    {
        _paused = true;
    }

    public void OnResume()
    {
        _paused = false;
    }
}
