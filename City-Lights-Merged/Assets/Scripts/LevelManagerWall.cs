using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerWall : MonoBehaviour
{

    public int level; // 0 = startscreen
    private AudioManagerWall audiomanager;
    private GameManagerWall gamemanager;
    private Fireworks fireworks;

    // Use this for initialization
    void Start()
    {
        level = 0;
        audiomanager = (AudioManagerWall)GameObject.FindObjectOfType<AudioManagerWall>();
        gamemanager = FindObjectOfType<GameManagerWall>();
        fireworks = GameObject.Find("Fireworks").GetComponent<Fireworks>();
        startAudio(0);
    }

    public void NextLevel()
    {
        level++;
        startAudio(level);
    }

    private void startAudio(int level)
    {
        switch (level)
        {
            case 0:
                StartCoroutine(Pause(0));
                audiomanager.Play("Rumble");
                StartCoroutine(audiomanager.PlayRandomOnce("Shiphorn1", 5, 15));
                StartCoroutine(audiomanager.PlayRandomRepeated("Shiphorn2", 25, 80));
                StartCoroutine(StartScreen(10));
                break;
            case 1:
                audiomanager.Play("Music1");
                audiomanager.Play("Bar1");
                StartCoroutine(audiomanager.PlayRandomRepeated("Shiphorn1", 40, 100));
                StartCoroutine(audiomanager.PlayRandomOnce("CatStory", 5, 15));
                break;
            case 2:
                audiomanager.Play("Bar2");
                StartCoroutine(audiomanager.PlayRandomOnce("CatStory", 20, 40));
                StartCoroutine(audiomanager.PlayRandomOnce("CarStory", 40, 80));
                break;
            case 3:
                audiomanager.Play("Music2");
                audiomanager.Play("Bar3");
                StartCoroutine(audiomanager.PlayRandomOnce("StreetCar", 5, 10));
                StartCoroutine(audiomanager.PlayRandomRepeated("StreetCar", 20, 80));
                StartCoroutine(audiomanager.PlayRandomOnce("Dog", 20, 100));
                break;
            case 4:
                StartCoroutine(audiomanager.PlayRandomOnce("HorseCarriage", 20, 50));
                break;
            case 5:
                StartCoroutine(audiomanager.PlayRandomOnce("Music3", 9, 9));
                StartCoroutine(StartFireworks(12.4f));
                StartCoroutine(StartBlackout(40f));
                StartCoroutine(StopFireworks(38f));

                break;
            default:
                break;
        }
    }

    public IEnumerator StartBlackout(float delay)
    {
        yield return new WaitForSeconds(delay);
        // stop music 3
        audiomanager.Stop("Music3");

        // play blackout audio
        audiomanager.Play("Blackout");

        // turn off lamps
        StartCoroutine(TurnLampOff(0.2f, "Port4-1"));
        StartCoroutine(TurnLampOff(0.4f, "Port4-2"));
        StartCoroutine(TurnLampOff(1.0f, "Port3-2"));
        StartCoroutine(TurnLampOff(0.8f, "Port3-1"));
        StartCoroutine(TurnLampOff(2.2f, "Port2-2"));
        StartCoroutine(TurnLampOff(2.5f, "Port2-1"));
        StartCoroutine(TurnLampOff(3.0f, "Port1-1"));
        StartCoroutine(TurnLampOff(3.4f, "Port1-2"));

        // short break (meaw, sigh, laugh?)
        StartCoroutine(audiomanager.PlayRandomOnce("Meow", 10, 13));

        // play sting
        StartCoroutine(audiomanager.PlayRandomOnce("Sting", 16, 16));

        // transist into endscreen
        StartCoroutine(Pause(15));

    }

    public IEnumerator TurnLampOff(float delay, string port)
    {
        yield return new WaitForSeconds(delay);

        Animator animator = GameObject.Find(port).GetComponent<Animator>();
        animator.SetBool("lightOn", false);
    }

    public IEnumerator StartFireworks(float delay)
    {
        yield return new WaitForSeconds(delay);

        fireworks.PlayFirework();
    }

    public IEnumerator StopFireworks(float delay)
    {
        yield return new WaitForSeconds(delay);

        fireworks.StopFirework();
    }

    public IEnumerator StartScreen(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!FindObjectOfType<GameManagerWall>().startscreen)
        {
            gamemanager.MainCam.GetComponent<Animator>().SetTrigger("nextLevel");
            NextLevel();
            Unpause();
        }
    }

    public IEnumerator Pause(float delay)
    {
        yield return new WaitForSeconds(delay);

        gamemanager.PauseScreen.GetComponent<Animator>().SetBool("pause", true);
        gamemanager.networker.GamePause(true);
    }

    public void Unpause()
    {
        gamemanager.PauseScreen.GetComponent<Animator>().SetBool("pause", false);
        gamemanager.networker.GamePause(false);
    }
}
