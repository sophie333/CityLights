using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour {
    public NetworkCommunicator communicator;
    public HostManager hostManager;
    public bool connected;
    //public //map lampName + pointLights
    //public //map lampName + lamp
    //lamps pointLight intensity von 0 auf 2
    //lamps setActive(true)

    private void Update()
    {
       this.connected = hostManager.connectedToClient;
    }

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

    public void PlayAudio(string trackName)
    {
        communicator.RpcPlayAudio(trackName);

    }

    public void GamePause(bool state)
    {
        communicator.CmdGamePause(state);
    }

    public void ReloadGame()
    {
        communicator.CmdReloadGame();
    }

    public void ReloadLevel()
    {
        communicator.CmdReloadLevel();
    }

    public void ToggleDebug()
    {
        communicator.CmdToggleDebug();
    }

    public void NextLevelServer()
    {
        communicator.CmdNextLevel();
    }
}
