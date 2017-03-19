﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour {

	public static int level = 0;
	public static int levelCap = 2;
	public static int enemiesRemaining = 0;
	public static GameObject levelContainer;
	public static GameObject hero, heroPrefab;

	void Start () {
		levelContainer = transform.FindChild ("LevelContainer").gameObject;
		heroPrefab = ResourceLoader.LoadPrefab ("Hero");
		InitHero ();
		LoadNextLevel ();
	}

	public static void InitHero() {
		hero = Instantiate (heroPrefab);
		Vector3 newPos = Camera.main.transform.position;
		newPos.z = 0;
		hero.transform.position = newPos;
	}

	public static void LoadNextLevel() {
		level++;
		UIManager.self.ShowLevelNumber (level);
		Destroy (levelContainer);
		levelContainer = Instantiate(ResourceLoader.LoadLevelPrefab (level), levelContainer.transform.parent);
	}

	public static void EnemyDied() {
		enemiesRemaining--;
		Debug.Log (enemiesRemaining);
		if (enemiesRemaining == 0 && level < levelCap) {
			LoadNextLevel ();
		}
	}

	public static void GameOver() {
		Notifications.self.SendGameOverNotification ();
	}

	public static void Restart() {
		Notifications.self.SendRestartNotification ();
		InitHero ();
		level = 0;
		enemiesRemaining = 0;
		LoadNextLevel ();
	}
}