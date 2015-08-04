using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatInputBehavior : MonoBehaviour {

	public ChatClientBehavior chatClient;
	InputField inField;

	void Start () {
		inField = GetComponent<InputField>();
	}
	
	public void OnEndEdit(string s) {
		if (chatClient.Send(s))
			inField.text = "";
			
		// by default, Unity releases focus on the inputfield after pressing enter
		inField.ActivateInputField(); // keep the focus
	}
}
