using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	private static GameObject menu;

	public void Start()
	{
		self = this;
		menu = transform.FindChild ("Menu").gameObject;
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
}
