using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public void OnEnable()
	{
		GameManager.PauseGame (true);
	}

	public void OnDisable()
	{
		GameManager.PauseGame (false);
	}

	public void ContinueLevel()
	{
		this.gameObject.SetActive (false);
	}
}
