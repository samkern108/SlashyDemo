using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPanel : MonoBehaviour {

	void Update () {
		if (Input.anyKeyDown) {
			LevelMaster.Restart ();
			gameObject.SetActive (false);
		}	
	}
}
