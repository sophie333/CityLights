using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : AManager<SocketManager>
{
    public enum ErrCode
    {
        NONE = 0,
        UNKNOWN = 1,
        END = 9999
    }
    enum MsgType
    {
        CONNECT_ACK = 11000,

        CREATE_USER_REQ = 11101,
        CREATE_USER_ACK = 11102,
    }

    class ConnectAck
    {
        public ErrCode errCode;
    }

    class CreateUserReq
    {
        public MsgType msgType = MsgType.CREATE_USER_REQ;

        public string userName;
        public string passwd;
    }

    class CreateUserAck
    {
        public ErrCode errCode;
    }
    //ws://echo.websocket.org
    //ws://quantumreboot.com/test/ws_echo
    public Text text;
    private string testmsg = "hello world";
    private string uri = "ws://quantumreboot.com/test/ws_echo";
    WebSocketClient ws;

    //StringBuilder sb;

    void Start()
    {
        Connect();
        InvokeRepeating("TestSend", 1f, 1f);
        InvokeRepeating("Receive", 1f,1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
           Send(testmsg);
        }
    }

    public void Connect()
    {
        ws = new WebSocketClient(new Uri(uri));
        ws.Connect();
    }

    void OnSendCompleted(bool result)
    {
        //print("send complete");
        return;
    }

    private void TestSend()
    {
        Send(testmsg);
    }

    public void Send(string message)
    {
        if (ws.isConnected == false)
        {
            return;
        }

        try
        {
            //var bytes = Encoding.UTF8.GetBytes(message);
            ws.Send(message, OnSendCompleted);
            print("Message Sent: " + message);
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    public void Receive()
    {
        var bytes = ws.Receive();
        if (bytes == null)
        {
            return;
        }
        var str = Encoding.UTF8.GetString(bytes);
        text.text = bytes.ToString();
        print("Received: " + str);
    }
}
