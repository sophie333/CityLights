using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : MonoBehaviour {

    private AudioManagerWall audiomanager;
    private ParticleSystem fireworks;
    private ParticleSystem.EmissionModule em;

    void Start () {
        audiomanager = FindObjectOfType<AudioManagerWall>();
        fireworks = GetComponent<ParticleSystem>();
        em = fireworks.emission;
	}

    public void PlayFirework()
    {
        fireworks.Play();
        StartCoroutine("DoEmit");
        Debug.Log("[Firework] started");
    }

    IEnumerator DoEmit()
    {
        while (true)
        {
            float interval = Random.Range(0.1f,0.5f);
            int amount = 1;
            em.rateOverTime = amount / interval;

            yield return new WaitForSeconds(interval);

            for (int i = 0; i < amount; i++)
            {
                audiomanager.PlayFireworkSound();
            }
        }
    }

    public void StopFirework()
    {
        fireworks.Stop();
        StopCoroutine("DoEmit");
    }
}
