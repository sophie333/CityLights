using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    public NetworkCommunicator communicator;
    //public //map lampName + pointLights
    //public //map lampName + lamp
    //lamps pointLight intensity von 0 auf 2
    //lamps setActive(true)

    public void TurnLightOn(string lampName)
    {
        communicator.RpcTurnLightOn(lampName);
    }

    public void TurnLightOff(string lampName)
    {
        communicator.RpcTurnLightOff(lampName);
    }

    public void AllLightsOff()
    {
        communicator.RpcTurnAllLightsOff();
    }

    public void NextLevel()
    {
        communicator.RpcNextLevel();
    }
}

