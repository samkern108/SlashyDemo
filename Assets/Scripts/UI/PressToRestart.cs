using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToRestart : MonoBehaviour {

	void Update () {
		if (LevelMaster.input.Continue()) {
			LevelMaster.Restart ();
			gameObject.SetActive (false);
		}	
	}
}
