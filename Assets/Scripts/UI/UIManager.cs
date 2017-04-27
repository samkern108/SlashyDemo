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
	private static GameObject highScoreRow;

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

	public void Start()
	{
		highScoreRow = ResourceLoader.LoadPrefab ("HighScoreRow");
		MakeHighScoreRows ();
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
		timerText.text = ScoreAsString (ScoreAsFloat());
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
		if (victory) {
			levelText.enabled = true;
			victoryPanel.SetActive (true);
			DisplayHighScores ();
		}
		else
			gameOverPanel.SetActive (true);
	}

	public void Respawn() {
		gameOverPanel.SetActive (false);
		victoryPanel.SetActive (false);		
	}

	public string scoreName = "SAM";

	public void UpdateScoreName(string name) {
		scoreName = name;
	}

	public void Restart() {
		levelText.enabled = true;
		gameOverPanel.SetActive (false);
		victoryPanel.SetActive (false);

		if (victoryPanel.transform.FindChild ("NameInput").gameObject.activeInHierarchy) {
			LevelMaster.SaveHighScoreName (0, scoreName);
			victoryPanel.transform.FindChild ("NameInput").gameObject.SetActive (false);
		}

		timing = true;
		timerMin = 0.0f;
		timerSec = 0.0f;
		timerText.text = "0:00:00";
	}

	public void LevelLoaded(int level) {
		UIManager.self.ShowLevelNumber (level);
		timing = true;
	}

	public void ShowInputHighScoreName (int index) {
		string name;
		victoryPanel.transform.FindChild ("NameInput").gameObject.SetActive (true);
	}

	private static int highScoreRowHeight = 30, highScoreXOffset = 60;

	private void MakeHighScoreRows() {
		GameObject row;
		int halfway = Mathf.CeilToInt (LevelMaster.highScoreTable.Length / 2);
		for (int i = 0; i < halfway; i++) {
			row = Instantiate (highScoreRow);
			row.name = "ScoreRow" + i;
			row.transform.SetParent (victoryPanel.transform.FindChild("ScoresTable"));
			row.transform.localPosition = new Vector3 (-highScoreXOffset, -(highScoreRowHeight * i));

			row = Instantiate (highScoreRow);
			row.name = "ScoreRow" + (i + halfway);
			row.transform.SetParent (victoryPanel.transform.FindChild("ScoresTable"));
			row.transform.localPosition = new Vector3 (highScoreXOffset, -(highScoreRowHeight * i));
		}
	}

	private void DisplayHighScores() {
		Transform table = victoryPanel.transform.FindChild ("ScoresTable");
		Transform row;
		for (int i = 0; i < LevelMaster.highScoreTable.Length; i++) {
			row = table.FindChild ("ScoreRow" + i).transform;
			row.transform.FindChild ("Score").GetComponent<Text>().text = "" + ScoreAsString(LevelMaster.highScoreTable[i]);
			row.transform.FindChild ("Name").GetComponent<Text>().text = LevelMaster.highScoreNames[i];
		}
	}

	public float ScoreAsFloat() {
		return UIManager.timerMin * 60 + UIManager.timerSec;
	}

	public string ScoreAsString(float scoreFloat) {
		int minutes = Mathf.FloorToInt(scoreFloat / 60);
		int seconds = Mathf.FloorToInt(scoreFloat % 60);
		int milliseconds = (int)(((scoreFloat % 60) - seconds) * 10);
			
		string minText = minutes + "";
		string secText = seconds + "";
		string millisText = milliseconds + "";

		if (timerMin < 10)
			minText = "0" + minText;
		if (timerSec < 10)
			secText = "0" + secText;

		return minText + ":" + secText + ":" + millisText;
	}
}
