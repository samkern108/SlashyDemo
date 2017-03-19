using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	private static GameObject menu;

	private static Text levelText, blueDotText;
	private static GameObject gameOver;

	public void Awake()
	{
		self = this;
		menu = transform.FindChild ("Menu").gameObject;
		levelText = transform.FindChild ("Level").GetComponent<Text>();
		blueDotText = transform.FindChild ("Blue Dots").GetComponent<Text>();
		gameOver = transform.FindChild ("Game Over").gameObject;
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			menu.SetActive (!menu.activeInHierarchy);
	}

	public void ShowVictoryPanel()
	{
		transform.FindChild ("VictoryPanel").gameObject.SetActive (true);
		GameManager.PauseGame (true);
	}

	public void ShowDefeatPanel()
	{
		transform.FindChild ("GameOverPanel").gameObject.SetActive (true);
		GameManager.PauseGame (true);
	}

	public void RetryButton()
	{
		GameManager.PauseGame (false);
		GameManager.RestartLevel ();
		menu.SetActive (false);
	}

	public void QuitButton()
	{
		GameManager.PauseGame (false);
		GameManager.QuitToMenu ();
	}

	public void ShowLevelNumber(int levelNumber)
	{
		levelText.text = "LEVEL " + levelNumber;
		levelText.FadeIn (2.0f, true);
	}

	public void SetBlueDots(int blueDots)
	{
		blueDotText.text = blueDots + "";
	}

	// Notifications

	public void GameOver() {
		gameOver.SetActive (true);
	}

	public void Restart() {
		gameOver.SetActive (false);
	}
}
