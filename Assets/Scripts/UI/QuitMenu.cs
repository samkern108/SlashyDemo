using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenu : MonoBehaviour {

	void Start () {
		// Set the text for the type of input we're taking.
	}

	void OnEnable() {
		Notifications.self.SendPauseNotification (true);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			gameObject.SetActive (false);
		}
		
		else if (Input.GetKeyDown (KeyCode.Q))
			Application.Quit ();

		else if (Input.GetKeyDown (KeyCode.Space)) {
			LevelMaster.Restart ();
			Notifications.self.SendPauseNotification (false);
			gameObject.SetActive (false);
		}
	}
}
