using System;
using System.Collections;
using UnityEngine;

public class AudioManagerWall : MonoBehaviour {

    public SoundSource[] wallSounds;
    public Sound[] floorSounds;
    public SoundSource[] fireworkSounds;

    // Use this for initialization
    void Awake()
    {
        foreach (SoundSource s in wallSounds)
        {
            s.source = s.object3D.GetComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.output;
            s.source.loop = s.loop;
        }
        foreach (Sound s in floorSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.outputAudioMixerGroup = s.output;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {

    }


    // Play Sound by name
    public void Play (String name)
    {
        SoundSource s = Array.Find(wallSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("[AudioManager] Couldn't find sound: " + name);
            return;  
        }
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
        
	}

    public void Stop (String name)
    {
        SoundSource s = Array.Find(wallSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("[AudioManager] Couldn't find sound: " + name);
            return;
        }
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    // Play sound once at a random point in a certain range of time
    public IEnumerator PlayRandomOnce(String name, float min, float max)
    {
        float random = UnityEngine.Random.Range(min, max);

        Debug.Log("[ONCE] Sound " + name + " will play in " + random + " seconds.");

        SoundSource s = Array.Find(wallSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("[AudioManager] Couldn't find sound: " + name);
            yield break;
        }

        yield return new WaitForSeconds(random);

        if (!s.source.isPlaying) // && level == LevelManagerWall.level) ?
        {
            s.source.Play();
            Debug.Log("Played " + name);
        }
    }

    // Play sound repeatedly, the interval between repetitions is random, but between min and max
    public IEnumerator PlayRandomRepeated(String name, float min, float max)
    {
        float random = UnityEngine.Random.Range(min, max);

        Debug.Log("[REPEATED] Sound " + name + " will play in " + random + " seconds.");
        SoundSource s = Array.Find(wallSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("[AudioManager] Couldn't find sound: " + name);
            yield break;
        }

        yield return new WaitForSeconds(random);

        if (!s.source.isPlaying) // && level == LevelManagerWall.level) ?
        {
            s.source.Play();
            Debug.Log("Played " + name);
            StartCoroutine(PlayRandomRepeated(name, min, max));
        }
    }

    // Play Floor Interaction Sound by name
    public void PlayFloor(String name)
    {
        Sound s = Array.Find(floorSounds, sound => sound.name == name);
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

    // Play a random firework sound
    public void PlayFireworkSound()
    {
        int random = UnityEngine.Random.Range(0, fireworkSounds.Length-1);

        SoundSource s = Array.Find(wallSounds, sound => sound.name == fireworkSounds[random].name);
        if (s == null)
        {
            Debug.LogError("[AudioManager] Couldn't find sound: " + fireworkSounds[random].name);
            return;
        }
        s.source.Play();
    }
}
