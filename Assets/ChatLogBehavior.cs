using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatLogBehavior : MonoBehaviour {

	Text textUI;
	public int maxLines = 10;
	int numLines = 0;

	void Awake() {
		textUI = GetComponent<Text>();
	}

	void OnEnable () {
		textUI.text = "";
		numLines = 0;
	}
	
	public void OnReceiveMessage(string s) {
		textUI.text += s + "\n";
		
		numLines++;
		if (numLines > maxLines) {
			// remove older lines lines
			string t = textUI.text;
			t = t.Substring(t.IndexOf('\n') + 1);
			textUI.text = t;
			numLines--;
		}
	}
}
