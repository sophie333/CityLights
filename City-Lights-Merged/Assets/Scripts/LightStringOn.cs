using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStringOn : MonoBehaviour {

    AudioSource audioData;

    private void Start()
    {
    }

    private void OnEnable()
    {
        audioData = GetComponent<AudioSource>();

        StartCoroutine(TurnBulbOn(transform));
        StartCoroutine(Playsound());
    }

    private IEnumerator TurnBulbOn(Transform lightString)
    {
        foreach (Transform child in transform)
        {
            yield return new WaitForSeconds(0.06f);
            child.GetComponent<Animator>().SetBool("lightOn", true);
        }
    }

    private IEnumerator Playsound()
    {
        yield return new WaitForSeconds(0.3f);
        audioData.Play();
    }
}
