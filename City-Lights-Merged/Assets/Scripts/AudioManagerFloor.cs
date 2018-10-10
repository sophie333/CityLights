using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManagerFloor : MonoBehaviour {

    public Sound[] sounds;

	// Use this for initialization
	void Awake () {
		foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.outputAudioMixerGroup = s.output;
            s.source.loop = s.loop;

        }
	}
	
	// Play Sound by name
	public void Play (String name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("[AudioManager] Couldn't find sound: " + name);
            return;  
        }
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }

    }
}
