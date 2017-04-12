using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	public static int level = 0;
	public static int levelCap = 3;
	public static int enemiesRemaining = 0;
	private static int blueDotsRemaining = 0;
	public static GameObject levelContainer;
	public static GameObject hero, heroPrefab;

	void Start () {
		IOManager.Initialize ();
		ParticleManager.Initialize ();
		AudioManager.Initialize ();

		levelCap = ResourceLoader.NumberOfLevels ();

		levelContainer = transform.FindChild ("LevelContainer").gameObject;
		heroPrefab = ResourceLoader.LoadPrefab ("Hero");
	}

	public static void InitHero() {
		if(hero) Destroy (hero);
		hero = Instantiate (heroPrefab);
		Vector3 newPos = Camera.main.transform.position;
		newPos.z = 0;
		hero.transform.position = newPos;
	}

	public static void LoadNextLevel() {
		level++;
		enemiesRemaining = 0;
		blueDotsRemaining = 0;

		Notifications.self.SendLoadLevelNotification(level);

		UIManager.self.ShowLevelNumber (level);
		Destroy (levelContainer);
		levelContainer = Instantiate(ResourceLoader.LoadLevelPrefab (level), levelContainer.transform.parent);
	}

	public static void EnemyDied() {
		enemiesRemaining--;
		if (enemiesRemaining == 0) {
			if (level < levelCap)
				LoadNextLevel ();
			else
				Victory ();
		}
	}

	// This is a silly way to do this
	public static void AddBlueDot() {
		blueDotsRemaining++;
		UIManager.self.SetBlueDots (blueDotsRemaining);
	}

	public static void CollectBlueDot() {
		blueDotsRemaining--;
		UIManager.self.SetBlueDots (blueDotsRemaining);
		if (blueDotsRemaining == 0) {
			if(level < levelCap)
				LoadNextLevel ();
			else
				Victory ();
		}
	}

	public static void Victory() {
		Notifications.self.SendGameEndNotification (true);
	}

	public static void Defeat() {
		Notifications.self.SendGameEndNotification (false);
	}

	public static void Restart() {
		Notifications.self.SendRestartNotification ();
		InitHero ();
		level = 0;
		LoadNextLevel ();
	}

	public static void Retry() {
		Notifications.self.SendRestartNotification ();
		InitHero ();
		level = Mathf.Max(level - 2, 0);
		LoadNextLevel ();
	}
}
