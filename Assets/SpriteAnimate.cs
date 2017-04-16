using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class SpriteAnimate : MonoBehaviour {

	private SpriteRenderer spriteR;
	private Vector3 originalScale;

	public void Awake() {
		originalScale = transform.localScale;
		spriteR = GetComponent <SpriteRenderer>();
	}

	public void ColorFade(Color start, Color end, float timer, bool autoreverses) {
		Timing.RunCoroutine (ColorCo(start, end, timer, autoreverses));
	}

	private IEnumerator<float> ColorCo (Color start, Color end, float timer, bool autoreverse) {
		float t = 0.0f;
		while (t < 1.0) {
			t += Time.deltaTime * (1.0f/timer);
			spriteR.color = Color.Lerp(start, end, t);
			yield return 0f;
		}
		if (autoreverse) {
			ColorFade (end, start, timer, false);
		}
	}

	//Applies a squish effect, and then unsquishes.
	public void Stretch(Vector3 deformation, float timer, bool playForward, bool autoreverses) {
		Vector3 stretchScale = originalScale;
		stretchScale.x *= (deformation.x);
		stretchScale.y *= (deformation.y);
		stretchScale.z *= (deformation.z);

		if(playForward) Timing.RunCoroutine (StretchCo(originalScale, stretchScale, timer, autoreverses));
		else Timing.RunCoroutine (StretchCo(stretchScale, originalScale, timer, autoreverses));
	}

	private IEnumerator<float> StretchCo (Vector3 originalScale, Vector3 stretchScale, float timer, bool autoreverse) {
		float t = 0.0f;
		while (t < 1.0) {
			t += Time.deltaTime * (1.0f/timer);
			transform.localScale = Vector3.Lerp (originalScale, stretchScale, t);
			yield return 0f;
		}
		transform.localScale = stretchScale;
		if (autoreverse) {
			Timing.RunCoroutine (StretchCo(stretchScale, originalScale, timer, false));
		}
	}
}
