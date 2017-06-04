using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToRetry : MonoBehaviour {

	void Update () {
		if (LevelMaster.input.Continue()) {
			LevelMaster.Respawn ();
			gameObject.SetActive (false);
		}	
	}
}
