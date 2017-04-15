using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToRetry : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			LevelMaster.Respawn ();
			gameObject.SetActive (false);
		}	
	}
}
