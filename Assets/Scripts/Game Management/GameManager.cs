using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static bool paused = false;

	public void Awake()
	{
		IOManager.Initialize ();
	}
		
	public void Start()
	{
		InitializeStaticObjects ();
		UpdateUI ();
	}

	private void InitializeStaticObjects()
	{
	}

	private void UpdateUI()
	{
	}

	public static void GameOver()
	{
		UIManager.self.ShowDefeatPanel ();	
	}
		
	public static void RestartLevel()
	{

	}

	public static void QuitToMenu()
	{

	}
		
	public static void PauseGame(bool pause)
	{
		paused = pause;
		if (pause) {
			Time.timeScale = 0f;
		} else {
			Time.timeScale = 1f;
		}
	}
}
