using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	public static int level = 0;
	public static int enemiesRemaining = 0;

	void Start () {
		LoadNextLevel ();
	}

	public static void LoadNextLevel() {
		level++;
		UIManager.self.ShowLevelNumber (level);
	}
}
