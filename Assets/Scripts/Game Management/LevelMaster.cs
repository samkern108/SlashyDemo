using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	/** DEBUGGING **/
	private static int levelCapOverride = 30;
	/** DEBUGGING **/

	public static int level = 1;
	public static int enemiesRemaining = 0;
	private static int blueDotsRemaining = 0;
	public static GameObject levelContainer;
	public static Transform inactiveLevelsParent;
	public static Transform lmTransform;
	public static GameObject hero, heroPrefab;

	public static float[] highScoreTable = new float[10];
	public static string[] highScoreNames = new string[10];

	// Hacky
	private static Vector3 playerRespawnPos = new Vector3(0,-3.88f,0);

	void Start () {
		lmTransform = this.transform;
		IOManager.Initialize ();
		ParticleManager.Initialize ();
		AudioManager.Initialize ();
		Palette.Initialize ();

		levelContainer = transform.Find ("LevelContainer").gameObject;
		inactiveLevelsParent = GameObject.Find ("Levels").transform;
		heroPrefab = ResourceLoader.LoadPrefab ("Hero");

		for(int i = 0; i < highScoreTable.Length; i++) {
			highScoreTable[i] = PlayerPrefs.GetFloat("HighScore" + i);
			highScoreNames[i] = PlayerPrefs.GetString("HighScoreName" + i);
		}
	}

	public static void InitHero(Vector3 spawnPosition) {
		if(hero) Destroy (hero);
		hero = Instantiate (heroPrefab, lmTransform);
		spawnPosition.z = 0;
		hero.transform.position = spawnPosition;
	}

	public static bool LoadNextLevel() {
		Transform levelObject = inactiveLevelsParent.Find ("Level " + level);//ResourceLoader.LoadLevelPrefab (level);
		if (!levelObject)
			return false;
		
		enemiesRemaining = 0;
		blueDotsRemaining = 0;

		Notifications.self.SendLoadLevelNotification(level);

		Destroy (levelContainer);
		levelContainer = Instantiate(levelObject.gameObject, levelContainer.transform.parent);
		levelContainer.SetActive (true);
		levelContainer.name = "LevelContainer";

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
		float score = UIManager.ScoreAsFloat();
		UpdateHighScores (score);
		Notifications.self.SendGameEndNotification (true);
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
		string nameTemp = "new";
		float scoreComp = score;
		float scoreCompTemp = score;
		for(int i = 0; i < highScoreTable.Length; i++) {
			if (scoreComp < highScoreTable [i] || highScoreTable [i] == 0) {
				// This is the player's high score!!
				// Color it yellow or some shit.
				if (name == "" && nameTemp == "new") {
					UIManager.ShowInputHighScoreName (i);
					nameTemp = "";
				} else {
					PlayerPrefs.SetString ("HighScoreName" + i, name);
					nameTemp = name;
					name = highScoreNames [i];
					highScoreNames [i] = nameTemp;
				}
				PlayerPrefs.SetFloat ("HighScore" + i,scoreComp);

				scoreCompTemp = scoreComp;
				scoreComp = highScoreTable [i];
				highScoreTable [i] = scoreCompTemp;

				dirty = true;
			}
		}
		if(dirty) PlayerPrefs.Save ();
	}

	public static void SaveHighScoreName (int index, string name) {
		PlayerPrefs.SetString ("HighScoreName" + index, name);
		PlayerPrefs.Save ();
	}
}
