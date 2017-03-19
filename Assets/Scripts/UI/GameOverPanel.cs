using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour {

	void Update () {
		if (Input.anyKeyDown) {
			LevelMaster.Restart ();
		}	
	}
}
