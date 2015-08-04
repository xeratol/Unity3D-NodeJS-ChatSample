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
	public GameObject [] onReceivedMessageListener;	// must have OnReceiveMessage(string)
	
	void Start () {
		foreach (GameObject go in activateOnConnect) {
			go.SetActive(false);
		}
		foreach (GameObject go in activateOnDisconnect) {
			go.SetActive(true);
		}
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
		
		foreach (GameObject go in activateOnConnect) {
			go.SetActive(true);
		}
		foreach (GameObject go in activateOnDisconnect) {
			go.SetActive(false);
		}
	}

	public bool Send(string msg) {
		if (tcpClient == null)
			return false;

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
		serverStream.Close();
		serverStream = null;
		tcpClient.Close();
		tcpClient = null;
		
		foreach (GameObject go in activateOnConnect) {
			go.SetActive(false);
		}
		foreach (GameObject go in activateOnDisconnect) {
			go.SetActive(true);
		}
	}

	void Update () {
		if (tcpClient == null || serverStream == null || !serverStream.DataAvailable)
			return;

		// Assumption: max message size is 1024
		byte [] msg = new byte[1024];
		int bytesRead = serverStream.Read(msg, 0, 1024);

		ASCIIEncoding encoder = new ASCIIEncoding();
		string message = encoder.GetString(msg, 0, bytesRead);
		
		foreach (GameObject go in onReceivedMessageListener) {
			go.SendMessage("OnReceiveMessage", message);
		}
	}
}