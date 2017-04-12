using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	private static GameObject menu;

	private static Text levelText, blueDotText;
	private static Text pressSpaceToSlash;
	private static GameObject gameOverPanel, victoryPanel;

	public void Awake()
	{
		self = this;
		menu = transform.FindChild ("Menu").gameObject;
		levelText = transform.FindChild ("Level").GetComponent<Text>();
		blueDotText = transform.FindChild ("Blue Dots").GetComponent<Text>();
		gameOverPanel = transform.FindChild ("Game Over").gameObject;
		victoryPanel = transform.FindChild ("Victory").gameObject;

		pressSpaceToSlash = transform.FindChild("Press Space To Slash").GetComponent<Text>();
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			menu.SetActive (!menu.activeInHierarchy);
	}

	public void ShowLevelNumber(int levelNumber)
	{
		levelText.text = "LEVEL " + levelNumber;
		levelText.FadeIn (2.0f, false);
	}

	public void SetBlueDots(int blueDots)
	{
		blueDotText.text = blueDots + "";
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
		if (victory)
			victoryPanel.SetActive (true);
		else
			gameOverPanel.SetActive (true);
	}

	public void Restart() {
		gameOverPanel.SetActive (false);
		victoryPanel.SetActive (false);
	}

	public void LevelLoaded(int level) {
		UIManager.self.ShowLevelNumber (level);
	}
}
