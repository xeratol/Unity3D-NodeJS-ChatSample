using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatLogBehavior : MonoBehaviour {

	Text textUI;
	public int maxLines = 10;
	int numLines = 0;

	void Start () {
		textUI = GetComponent<Text>();
		textUI.text = "";
	}
	
	public void OnReceiveMessage(string s) {
		textUI.text += s + "\n";
		
		numLines++;
		if (numLines > maxLines) {
			// remove previous lines
			string t = textUI.text;
			t = t.Substring(t.IndexOf('\n') + 1);
			textUI.text = t;
		}
	}
}
