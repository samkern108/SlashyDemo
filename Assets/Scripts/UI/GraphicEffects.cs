using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MovementEffects;
using System.Collections.Generic;

public enum EaseType {None, In, Out, InOut}

public static class GraphicEffects {

	//Adapted from Eric Haines' Fade.js
	//http://wiki.unity3d.com/index.php/Fade

	public static void Flicker(this Graphic obj)
	{
		Timing.RunCoroutine (FlickerCo(obj));
	}

	private static IEnumerator<float> FlickerCo(this Graphic obj) {
		float timer = 2000;
		float t = 0;
		while (true) {
			Color c = obj.color;
			c.a += Random.Range (1f - c.a, .6f - c.a);
			obj.color = c;
			yield return 0f;
		}
	}	

	public static void AlphaFade(this Graphic obj, float start, float end, float timer, EaseType easeType, bool autoreverse)
	{
		Timing.RunCoroutine (AlphaCo(obj, start, end, timer, easeType, autoreverse));
	}

	public static void ColorFade(this Graphic obj, Color start, Color end, float timer, EaseType easeType, bool autoreverse)
	{
		Timing.RunCoroutine (ColorCo(obj, start, end, timer, easeType, autoreverse));
	}

	public static void ColorsFade(this Graphic obj, Color[] colorRange, float timer, bool repeat)
	{
		Timing.RunCoroutine (ColorsCo(obj, colorRange, timer, repeat));
	}

	private static IEnumerator<float> AlphaCo (this Graphic obj, float start, float end, float timer, EaseType easeType, bool autoreverse) {
		float t = 0.0f;
		while (t < 1.0) {
			t += Time.deltaTime * (1.0f/timer);
			Color c = obj.color;
			c.a = Mathf.Lerp(start, end, Ease(t, easeType));
			obj.color = c;
			yield return 0f;
		}
		if (autoreverse) {
			AlphaFade (obj, end, start, timer, easeType, false);
		}
	}

	private static IEnumerator<float> ColorCo (this Graphic obj, Color start, Color end, float timer, EaseType easeType, bool autoreverse) {
		float t = 0.0f;
		while (t < 1.0) {
			t += Time.deltaTime * (1.0f/timer);
			obj.color = Color.Lerp(start, end, Ease(t, easeType));
			yield return 0f;
		}
		if (autoreverse) {
			ColorFade (obj, end, start, timer, easeType, false);
		}
	}

	private static IEnumerator<float> ColorsCo (this Graphic obj, Color[] colorRange, float timer, bool repeat) {
		if (colorRange.Length < 2) {
			Debug.LogError("Error: color array must have at least 2 entries");
			yield break;
		}
		timer /= colorRange.Length;
		int i = 0;
		while (true) {
			float t = 0.0f;
			while (t < 1.0f) {
				t += Time.deltaTime * (1.0f/timer);
				obj.color = Color.Lerp(colorRange[i], colorRange[(i+1) % colorRange.Length], t);
				yield return 0f;
			}
			i = ++i % colorRange.Length;
			if (!repeat && i == 0) break;
		}	
	}

	private static float Ease (float t, EaseType easeType) {
		if (easeType == EaseType.None)
			return t;
		else if (easeType == EaseType.In)
			return Mathf.Lerp(0.0f, 1.0f, 1.0f - Mathf.Cos(t * Mathf.PI * .5f));
		else if (easeType == EaseType.Out)
			return Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(t * Mathf.PI * .5f));
		else
			return Mathf.SmoothStep(0.0f, 1.0f, t);
	}

	public static void FadeIn(this Graphic obj, float timer, bool autoreverse)
	{
		AlphaFade (obj, 0.0f, 1.0f, timer, EaseType.None, autoreverse);
	}

	public static void FadeOut(this Graphic obj, float timer, bool autoreverse)
	{
		AlphaFade (obj, 1.0f, 0.0f, timer, EaseType.None, autoreverse);
	}
}
