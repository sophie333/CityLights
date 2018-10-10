using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityTuio;
using UnityEngine.SceneManagement;

public class HostManager : NetworkManager
{

    public static HostManager Instance = null;
    public string ConnectionIp;
    public bool connectedToClient;
    private bool server;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        server = true;
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
        /* if (Settings.Instance.LoadedData.NetworkType.Equals("Client"))
         {
             SceneManager.activeSceneChanged += ChangedActiveScene;
         }*/
    }

    /* private void ChangedActiveScene(Scene current, Scene next)
     {
         Settings.Instance.lo = "Server";

         Debug.Log("Scenes: " + current.name + ", " + next.name);
     }*/

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!server)
        {
            Debug.Log("Client connected!");
            connectedToClient = true;
        }
        server = false;

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        connectedToClient = false;
        base.OnClientDisconnect(conn);
        CleanUp();
        Invoke("Reconnect", 2f);
    }

    void OnDestroy()
    {
        connectedToClient = false;
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
