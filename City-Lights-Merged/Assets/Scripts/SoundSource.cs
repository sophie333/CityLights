using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class SoundSource {

    public string name;

    public GameObject object3D;

    [HideInInspector]
    public AudioSource source;

    public AudioMixerGroup output;

    public bool loop;

}
