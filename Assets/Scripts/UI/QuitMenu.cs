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
		if (LevelMaster.input.MenuExit()) {
			gameObject.SetActive (false);
		}
		
		else if (Input.GetKeyDown (KeyCode.Q))
			Application.Quit ();

		else if (LevelMaster.input.Continue()) {
			LevelMaster.Restart ();
			Notifications.self.SendPauseNotification (false);
			gameObject.SetActive (false);
		}
	}
}
