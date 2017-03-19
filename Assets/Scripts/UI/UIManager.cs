using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	private static GameObject menu;

	private static Text levelText, blueDotText;
	private static GameObject gameOverPanel, victoryPanel;

	public void Awake()
	{
		self = this;
		menu = transform.FindChild ("Menu").gameObject;
		levelText = transform.FindChild ("Level").GetComponent<Text>();
		blueDotText = transform.FindChild ("Blue Dots").GetComponent<Text>();
		gameOverPanel = transform.FindChild ("Game Over").gameObject;
		victoryPanel = transform.FindChild ("Victory").gameObject;
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
}
