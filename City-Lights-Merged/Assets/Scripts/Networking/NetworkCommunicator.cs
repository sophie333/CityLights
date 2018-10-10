using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkCommunicator : NetworkBehaviour
{
    public GameObject[] lights;
    public GameObject helper;
    public AudioManagerWall audiomanager;

    // Use this for initialization
    void Start()
    {
        if (!isServer)
        {
            //set lights
            lights = GameObject.FindGameObjectsWithTag("StreetLight");

            audiomanager = (AudioManagerWall)GameObject.FindObjectOfType<AudioManagerWall>();

        }

        if (isLocalPlayer)
        {
            helper = GameObject.Find("NetworkHelper");
            helper.GetComponent<NetworkHelper>().communicator = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ClientRpc]
    public void RpcTurnLightOn(string lampName) //string light type as parameter
    {
        if (!isServer)
        {
            Debug.Log("Turn light " + lampName + " on.");

            foreach (GameObject lamp in lights)
            {
                if (lamp.name == lampName)
                {
                    Animator animator = lamp.GetComponent<Animator>();
                    animator.SetBool("lightOn", true);

                    return;
                }
            }
            Debug.Log("Light " + lampName + " not found.");
        }
    }

    [ClientRpc]
    public void RpcTurnLightOff(string lampName) //string light type as parameter
    {
        if (!isServer)
        {
            Debug.Log("Turn light " + lampName + " off.");

            foreach (GameObject lamp in lights)
            {
                if (lamp.name == lampName)
                {
                    Animator animator = lamp.GetComponent<Animator>();
                    animator.SetBool("lightOn", false);

                    return;
                }
            }
            Debug.Log("Light " + lampName + " not found.");
        }
    }

    [ClientRpc]
    public void RpcTurnAllLightsOff()
    {
        if (!isServer)
        {
            Debug.Log("Turn all lights off.");
            foreach (GameObject lamp in lights)
            {
                Animator animator = lamp.GetComponent<Animator>();
                animator.SetBool("lightOn", false);

                return;
            }
        }
    }

    //spawn finish system prefab, which manages everything on the client
    [ClientRpc]
    public void RpcInstantiateAnimator()
    {
        if (!isServer)
        {
            Debug.Log("Instantiate Animator.");
        }
    }

    //start animation to next level
    [ClientRpc]
    public void RpcNextLevel()
    {
        if (!isServer)
        {
            Debug.Log("Go to next Level.");
            GameObject.FindObjectOfType<GameManagerWall>().MainCam.GetComponent<Animator>().SetTrigger("nextLevel");
            GameObject.FindObjectOfType<LevelManagerWall>().NextLevel();
        }
    }

    //play audio
    [ClientRpc]
    public void RpcPlayAudio(string clipName)
    {
        if (!isServer)
        {
            audiomanager.PlayFloor(clipName);

        }
    }
    
    //Pause and unpause the game
    [Command]
    public void CmdGamePause(bool state)
    {
        if (isServer)
        {
            Debug.Log("Pause!");
            FindObjectOfType<GameManagerFloor>().pauseOverlay.GetComponent<Animator>().SetBool("pause", state);
            FindObjectOfType<GameManagerFloor>().gamePaused = state;
        }
    }

    //Go to next level on floor
    [Command]
    public void CmdNextLevel()
    {
        if (isServer)
        {
            FindObjectOfType<GameManagerFloor>().LoadNextScene();
        }
    }

    //Reload game
    [Command]
    public void CmdReloadGame()
    {
        if (isServer)
        {
            FindObjectOfType<GameManagerFloor>().ReloadGame();
        }
    }

    //Reload current level
    [Command]
    public void CmdReloadLevel()
    {
        if (isServer)
        {
            FindObjectOfType<GameManagerFloor>().ReloadLevel();
        }
    }

    [Command]
    public void CmdToggleDebug()
    {
        if (isServer)
        {
            GameManagerFloor GMFloor = FindObjectOfType<GameManagerFloor>();
            GMFloor.DebugText.enabled = !GMFloor.DebugText.enabled;
            GMFloor.FPSText.enabled = !GMFloor.FPSText.enabled;
        }
    }
}
