using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLampOnSound : MonoBehaviour {

    AudioSource audioData;

    private void Start()
    {
    }

    private void OnEnable()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play();
        Debug.Log("played lamp on audio");
    }
}
