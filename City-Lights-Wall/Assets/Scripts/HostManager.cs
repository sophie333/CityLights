using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostManager : NetworkManager
{

    public static HostManager Instance = null;
    public string ConnectionIp;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        if (Settings.Instance.LoadedData.NetworkType.Equals("Client"))
        {
            //GameManager.Instance.EnableWall(); //Instead Instance the GameManager for the Wall
            networkAddress = Settings.Instance.LoadedData.ServerIp;
            networkPort = Settings.Instance.LoadedData.Port;
            StartClient();
        }
        if (Settings.Instance.LoadedData.NetworkType.Equals("Server"))
        {
            //GameManager.Instance.EnableFloor(); //Instead Instance the GameManager for the Floor
            networkPort = Settings.Instance.LoadedData.Port;
            StartHost();
        }
    }

    void Update()
    {

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        CleanUp();
        Invoke("Reconnect", 2f);
    }

    void OnDestroy()
    {
        CleanUp();
    }

    void Reconnect()
    {
        networkAddress = Settings.Instance.LoadedData.ServerIp;
        networkPort = Settings.Instance.LoadedData.Port;
        StartClient();
    }

    void CleanUp()
    {
        StopHost();
        StopClient();
        NetworkServer.ClearLocalObjects();
        NetworkServer.ClearSpawners();
        NetworkServer.Reset();
    }
}
