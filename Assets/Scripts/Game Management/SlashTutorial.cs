using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashTutorial : MonoBehaviour {

	void Start () {
		Invoke ("DisplayTutorialText", 2.0f);
	}
	
	private void DisplayTutorialText () {
		UIManager.DisplaySpaceToSlash (true);
	}

	public void N_LevelLoaded(int level) {
		UIManager.DisplaySpaceToSlash (false);
	}
}
