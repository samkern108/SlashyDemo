using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	/** DEBUGGING **/
	private static int levelCapOverride = 100;
	/** DEBUGGING **/

	public static int level = 1;
	public static int enemiesRemaining = 0;
	private static int blueDotsRemaining = 0;
	public static GameObject levelContainer;
	public static GameObject hero, heroPrefab;

	public static float[] highScoreTable = new float[10];
	public static string[] highScoreNames = new string[10];

	private static Vector3 playerRespawnPos = new Vector3(0,0,0);

	void Start () {
		IOManager.Initialize ();
		ParticleManager.Initialize ();
		AudioManager.Initialize ();

		levelContainer = transform.FindChild ("LevelContainer").gameObject;
		heroPrefab = ResourceLoader.LoadPrefab ("Hero");

		for(int i = 0; i < highScoreTable.Length; i++) {
			highScoreTable[i] = PlayerPrefs.GetFloat("HighScore" + i);
			highScoreNames[i] = PlayerPrefs.GetString("HighScoreName" + i);
		}
	}

	public static void InitHero(Vector3 spawnPosition) {
		if(hero) Destroy (hero);
		hero = Instantiate (heroPrefab);
		spawnPosition.z = 0;
		hero.transform.position = spawnPosition;
	}

	public static bool LoadNextLevel() {
		GameObject levelObject = ResourceLoader.LoadLevelPrefab (level);
		if (!levelObject)
			return false;
		
		enemiesRemaining = 0;
		blueDotsRemaining = 0;

		Notifications.self.SendLoadLevelNotification(level);

		Destroy (levelContainer);
		levelContainer = Instantiate(levelObject, levelContainer.transform.parent);

		level++;

		if (level > levelCapOverride)
			Victory ();

		return true;
	}

	public static void EnemyDied() {
		enemiesRemaining--;
		if (enemiesRemaining == 0) {
			if (!LoadNextLevel ())
				Victory ();
		}
	}

	// This is a silly way to do this
	public static void AddBlueDot() {
		blueDotsRemaining++;
	}

	public static void CollectBlueDot(Vector3 blueDotPosition) {
		blueDotsRemaining--;
		if (blueDotsRemaining == 0) {
			playerRespawnPos = (Vector2)blueDotPosition;
			if (!LoadNextLevel ())
				Victory ();
		}
	}

	public static void Victory() {
		Notifications.self.SendGameEndNotification (true);
		float score = UIManager.timerMin * 60 + UIManager.timerSec;
		UpdateHighScores (score);
	}

	public static void Defeat() {
		Notifications.self.SendGameEndNotification (false);
	}

	public static void Restart() {
		Notifications.self.SendRestartNotification ();
		InitHero (playerRespawnPos);
		level = 1;
		LoadNextLevel ();
	}

	public static void Respawn() {
		Notifications.self.SendRespawnNotification ();
		InitHero (playerRespawnPos);
		level = level - 1;
		LoadNextLevel ();
	}

	private static void UpdateHighScores(float score) {
		bool dirty = false;
		string name = "";
		string nameTemp = "";
		float scoreComp = score;
		float scoreCompTemp = score;
		for(int i = 0; i < highScoreTable.Length; i++) {
			if (scoreComp < highScoreTable [i]) {
				PlayerPrefs.SetFloat ("HighScore" + i,scoreComp);
				PlayerPrefs.SetString ("HighScoreName" + i, name);

				scoreCompTemp = scoreComp;
				scoreComp = highScoreTable [i];
				highScoreTable [i] = scoreCompTemp;

				nameTemp = name;
				name = highScoreNames [i];
				highScoreNames [i] = nameTemp;

				dirty = true;
			}
		}
		if(dirty) PlayerPrefs.Save ();
	}
}
