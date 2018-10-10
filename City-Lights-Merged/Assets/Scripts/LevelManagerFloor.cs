using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LevelManagerFloor : MonoBehaviour {

    public string levelName;
    public Network_LampPort Lamp1;
    public Network_LampPort Lamp2;

    public NetworkHelper networker;

    public Image black;
    public Animator anim;

    private bool first = true;


    // Use this for initialization
    void Start ()
    {
        first = true;
	}

    private void Awake()
    {
        if (levelName == "Win")
        {
            Invoke("ColorsOff", 37.5f);
        }
    }

    //Turn Colors in Floor-Win off.
    void ColorsOff()
    {
        GameObject.Find("Colors").GetComponent<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Lamp1.rightInputColor == true && Lamp2.rightInputColor == true)
        {
            {
                StartCoroutine(Count());
            }
        }
    }

    private IEnumerator Count()
    {
        yield return new WaitForSeconds(1f);

        if (Lamp1.rightInputColor == true && Lamp2.rightInputColor == true && first)
        {
            //send information to client (wall)
            networker.NextLevel();

            StartCoroutine(Transition());

            first = false;
        }
    }

    public IEnumerator Transition()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSeconds(2.6f);

        if (levelName == "LevelOne")
        {
            SceneManager.LoadScene("Level02");
        }
        else if (levelName == "LevelTwo")
        {
            SceneManager.LoadScene("Level03");
        }
        else if (levelName == "LevelThree")
        {
            SceneManager.LoadScene("Level04");
        }
        else if (levelName == "LevelFour")
        {
            SceneManager.LoadScene("Floor-Win");
        }
    }
}
