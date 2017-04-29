﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashTutorial : MonoBehaviour {

	void Start () {
		Invoke ("DisplayTutorialText", 2.0f);
	}
	
	private void DisplayTutorialText () {
		UIManager.self.DisplaySpaceToSlash (true);
	}

	public void LevelLoaded(int level) {
		UIManager.self.DisplaySpaceToSlash (false);
	}
}
