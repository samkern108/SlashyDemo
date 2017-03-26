using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToRestart : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			LevelMaster.Restart ();
			gameObject.SetActive (false);
		}	
	}
}
