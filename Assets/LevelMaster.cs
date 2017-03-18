using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	public static int level = 0;
	public static int enemiesRemaining = 0;
	public static GameObject levelContainer;

	void Start () {
		levelContainer = transform.FindChild ("LevelContainer").gameObject;
		LoadNextLevel ();
	}

	public static void LoadNextLevel() {
		level++;
		UIManager.self.ShowLevelNumber (level);
		levelContainer = Instantiate(ResourceLoader.LoadLevelPrefab (level));
	}
}
