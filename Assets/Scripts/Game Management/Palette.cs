using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Palette {

	public static Gradient slashLineRed = new Gradient();
	public static Gradient slashLineYellow = new Gradient();

	public static void Initialize() {
		slashLineRed.mode = GradientMode.Blend;
		slashLineYellow.mode = GradientMode.Blend;

		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
		alphaKeys [0].alpha = 0.0f;
		alphaKeys [1].alpha = 1.0f;
		alphaKeys [0].time = 0.0f;
		alphaKeys [1].time = 1.0f;

		slashLineRed.alphaKeys = alphaKeys;
		slashLineYellow.alphaKeys = alphaKeys;

		GradientColorKey[] yColorKeys = new GradientColorKey[2];
		yColorKeys [0].color = new Color (1f, 142.0f/255, 0f);
		yColorKeys [1].color = new Color (230.0f/255, 1f, 0f);
		yColorKeys [0].time = 0.0f;
		yColorKeys [1].time = .42f;

		GradientColorKey[] rColorKeys = new GradientColorKey[2];
		rColorKeys [0].color = new Color (208.0f/255, 112.0f/255, 90.0f/255);
		rColorKeys [1].color = new Color (203.0f/255, 115.0f/255, 48.0f/255);
		rColorKeys [0].time = 0.0f;
		rColorKeys [1].time = .42f;

		slashLineRed.colorKeys = rColorKeys;
		slashLineYellow.colorKeys = yColorKeys;
	}
}
