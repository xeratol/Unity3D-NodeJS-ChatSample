using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Net.Sockets;

public class ChatClientBehavior : MonoBehaviour {

	TcpClient tcpClient;
	NetworkStream serverStream;

	public string host;
	public int port;

	public GameObject [] activateOnConnect; 		// disabled on disconnect
	public GameObject [] activateOnDisconnect; 		// disabled on connect
	public GameObject [] onReceivedMessageListeners;// must have OnReceiveMessage(string)
	public GameObject [] onConnectListeners;		// must have OnConnect()
	public GameObject [] onDisconnectListeners;		// must have OnDisconnect()
	
	void Awake () {
		ActivateDeactivateObjects (false);
	}

	public void Connect () {
		tcpClient = new TcpClient();
		try {
			tcpClient.Connect(host, port);
		} catch (SocketException e) {
			Debug.LogError(e.ErrorCode + ": " + e.Message);
			tcpClient = null; // FIXME should I dispose first?
			return;
		}
		serverStream = tcpClient.GetStream();
		
		ActivateDeactivateObjects (true);

		foreach (GameObject go in onConnectListeners) {
			go.SendMessage("OnConnect");
		}
	}

	public bool Send(string msg) {
		if (tcpClient == null) // can't send if we're disconnected
			return false;

		//Debug.Log (msg);

		byte [] outstream = Encoding.ASCII.GetBytes(msg);
		try {
			serverStream.Write(outstream, 0, outstream.Length);
		} catch (IOException e) {
			Debug.LogError(e.Message);
			Disconnect();
			return false;
		}
		
		return true;
	}
	
	public void Disconnect () {
		foreach (GameObject go in onDisconnectListeners) {
			go.SendMessage("OnDisconnect");
		}

		ActivateDeactivateObjects (false);

		serverStream.Close();
		serverStream = null;
		tcpClient.Close ();
		tcpClient = null;
	}

	void Update () {
		if (tcpClient == null || serverStream == null || !serverStream.DataAvailable)
			return;

		// Assumption: max message size is 1024 characters
		byte [] msg = new byte[1024];
		int bytesRead = serverStream.Read(msg, 0, 1024);

		ASCIIEncoding encoder = new ASCIIEncoding();
		string message = encoder.GetString(msg, 0, bytesRead);
		
		foreach (GameObject go in onReceivedMessageListeners) {
			go.SendMessage("OnReceiveMessage", message);
		}
	}

	void ActivateDeactivateObjects (bool connected) {
		foreach (GameObject go in activateOnConnect) {
			go.SetActive(connected);
		}
		foreach (GameObject go in activateOnDisconnect) {
			go.SetActive(!connected);
		}
	}

	void OnApplicationQuit () {
		if (tcpClient != null)
			Disconnect (); // just in case client exits before clicking disconnect button
	}
}