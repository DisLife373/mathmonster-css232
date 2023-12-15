using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;

    private Sound current_sound;

    void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }
    }

    public void PlaySound(string _name) {
        Sound s = Array.Find(sounds, sound => sound.name == _name);

        if (s == null) {
            Debug.LogWarning("Sound: " + _name + " not found.");
            return;
        } 

        if (current_sound == null && s.name.Contains("BG")) {
            s.source.Play();
            current_sound = s;
        }
        else if (current_sound != null && s != current_sound && s.name.Contains("BG")) {
            s.source.Play();
            current_sound.source.Stop();
            current_sound = s;
        }
        else {
            s.source.Play();
        }
        
        
        
    }
}
