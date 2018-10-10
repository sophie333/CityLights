using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// FLOOR PROJECT VERSION
// (AudioManagerauskommentiert, aktuelle Version im Wall Projekt)


public class NetworkCommunicator : NetworkBehaviour
{
    public GameObject[] lights;
    public GameObject helper;
 //   public AudioManagerWall audiomanager;

    // Use this for initialization
    void Start()
    {
        if (!isServer)
        {
            //set lights
            lights = GameObject.FindGameObjectsWithTag("StreetLight");
            //TODO schön programmieren: GameManager.Instance.GetComponent<>

            //audiomanager = (AudioManagerWall)GameObject.FindObjectOfType<AudioManagerWall>();
        }

        helper = GameObject.Find("NetworkHelper");
        helper.GetComponent<NetworkHelper>().communicator = this;
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
                    // enable the colored lamp mesh
                    lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = true;

                    // enable the associated light source
                    lamp.transform.Find("light").GetComponent<Light>().enabled = true;

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
                    // enable the colored lamp mesh
                    lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = false;

                    // enable the associated light source
                    lamp.transform.Find("light").GetComponent<Light>().enabled = false;

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
                // enable the colored lamp mesh
                lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = false;

                // enable the associated light source
                lamp.transform.Find("light").GetComponent<Light>().enabled = false;
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
        }
    }

    //play audio
    [ClientRpc]
    public void RpcPlayAudio(string clipName)
    {
        if (!isServer)
        {
            //audiomanager.Play(clipName);

        }

    }

}
