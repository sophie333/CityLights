
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WebSocketSharp;

public class WebSocketClient
{
	public bool isConnected = false;
	public string error = null;

	Uri uri;
	WebSocket webSocket;
	Queue<byte[]> messages = new Queue<byte[]>();

	public WebSocketClient(Uri uri)
	{
		this.uri = uri;
	}

	public void Connect()
	{
		webSocket = new WebSocket(uri.ToString());
        
		{
			webSocket.OnOpen += (sender, e) => isConnected = true;
			webSocket.OnMessage += (sender, e) => messages.Enqueue(e.RawData);
			webSocket.OnError += (sender, e) => error = e.Message;
			webSocket.OnClose += (sender, e) => isConnected = false;
			webSocket.ConnectAsync();
		}
	}

    public void Send(string data, Action<bool> onSendCompleted)
    {
        webSocket.SendAsync(data, onSendCompleted);
    }
	//public void Send(byte[] msg, Action<bool> onSendCompleted)
	//{
	//	webSocket.SendAsync(msg, onSendCompleted);
	//}

	public byte[] Receive()
	{
		if (messages.Count == 0)
		{
			return null;
		}

		return messages.Dequeue();
	}

	public void Close()
	{
		webSocket.CloseAsync();
	}
}
