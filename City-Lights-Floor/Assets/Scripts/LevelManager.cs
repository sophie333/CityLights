using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    public string levelName;
    public Network_LampPort Lamp1;
    public Network_LampPort Lamp2;

    public NetworkHelper networker;

    public Image black;
    public Animator anim;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Lamp1.rightInputColor == true && Lamp2.rightInputColor == true)
        {
            //send information to client (wall)
            networker.NextLevel();

            StartCoroutine(Transition());        

            if (levelName == "LevelOne")
            {
                SceneManager.LoadScene("Level02");
            }
            else if (levelName == "LevelTwo")
            {
                SceneManager.LoadScene("Level03");
            }
            else if (levelName == "LevelTwo")
            {
                SceneManager.LoadScene("Level03");
            }
            else if (levelName == "LevelThree")
            {
                SceneManager.LoadScene("Level04");
            }

            /*
            GameObject level1 = GameObject.Find("Level01");
            level1.SetActive(false);
            Transform[] trans = GameObject.Find("Level02").GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans)
            {
                t.gameObject.SetActive(true);
            }*/

        }
	}
    IEnumerator Transition()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(()=>black.color.a==1);
    }
}
