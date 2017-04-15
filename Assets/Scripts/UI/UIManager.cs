using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	private static GameObject menu;

	public static float timerMin = 0.0f;
	public static float timerSec = 0.0f;

	private static bool timing = false;
	private static Text levelText, timerText;
	private static Text pressSpaceToSlash;
	private static GameObject gameOverPanel, victoryPanel;

	public void Awake()
	{
		self = this;
		menu = transform.FindChild ("Menu").gameObject;
		levelText = transform.FindChild ("Level").GetComponent<Text>();
		timerText = transform.FindChild ("Timer").GetComponent<Text>();
		gameOverPanel = transform.FindChild ("Game Over").gameObject;
		victoryPanel = transform.FindChild ("Victory").gameObject;

		pressSpaceToSlash = transform.FindChild("Press Space To Slash").GetComponent<Text>();
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			menu.SetActive (!menu.activeInHierarchy);
		// Does this keep time accurately enough? Should we subtract time from time started?
		if (timing) {
			timerSec += Time.deltaTime;

			if (timerSec >= 60) {
				timerSec -= 60;
				timerMin++;
			}

			UpdateTimerText ();
		}
	}

	public void ShowLevelNumber(int levelNumber)
	{
		levelText.text = "LEVEL " + levelNumber;
		levelText.FadeIn (2.0f, false);
	}

	public void UpdateTimerText()
	{
		string minText = timerMin + "";
		int floorSeconds = (int)Mathf.Floor (timerSec);
		string secText = floorSeconds + "";
		int milis = (int)((timerSec - floorSeconds) * 10);
		string milisText = milis + "";

		if (timerMin < 10)
			minText = "0" + minText;
		if (timerSec < 10)
			secText = "0" + secText;
			
		timerText.text = minText + ":" + secText + ":" + milisText;
	}

	public void DisplaySpaceToSlash(bool display)
	{
		pressSpaceToSlash.gameObject.SetActive (display);
		if(display) pressSpaceToSlash.Flicker ();
	}

	public void HideSpaceToSlash()
	{
		pressSpaceToSlash.gameObject.SetActive (false);
	}

	// Notifications

	public void GameEnd(bool victory) {
		timing = false;
		if (victory)
			victoryPanel.SetActive (true);
		else
			gameOverPanel.SetActive (true);
	}

	public void Respawn() {
		gameOverPanel.SetActive (false);
		victoryPanel.SetActive (false);		
	}

	public void Restart() {
		gameOverPanel.SetActive (false);
		victoryPanel.SetActive (false);

		timing = true;
		timerMin = 0.0f;
		timerSec = 0.0f;
		timerText.text = "0:00:00";
	}

	public void LevelLoaded(int level) {
		UIManager.self.ShowLevelNumber (level);
		timing = true;
	}
}
