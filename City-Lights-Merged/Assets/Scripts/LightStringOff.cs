using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStringOff : MonoBehaviour {

    AudioSource audioData;

    private void Start()
    {
    }

    private void OnEnable()
    {
        StartCoroutine(TurnBulbOff(transform));
    }

    private IEnumerator TurnBulbOff(Transform lightString)
    {
        foreach (Transform child in transform)
        {
            yield return new WaitForSeconds(0.06f);
            child.GetComponent<Animator>().SetBool("lightOn", false);
        }
    }
}
