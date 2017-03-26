using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public static class Screenshake {

	private static Vector3 cameraStartPosition;
	private static bool shake = false;

	public static void ScreenShake(this Camera camera, float severityX, float severityY) {
		Timing.RunCoroutine (TimedShake(camera, severityX, severityY));
	}

	public static void StartShake(this Camera camera, float severityX, float severityY) {
		shake = true;
		Timing.RunCoroutine (TimedShake(camera, severityX, severityY));
	}

	public static void EndShake(this Camera camera) {
		shake = false;
		camera.transform.position = cameraStartPosition;
	}

	public static void LockCamera(this Camera camera) {
		cameraStartPosition = camera.transform.position;
	}

	public static void SingleShake(this Camera camera, float severityX, float severityY) {
		float quakeAmtX = UnityEngine.Random.value * Mathf.Sin(severityX)*2 - severityX;
		float quakeAmtY = UnityEngine.Random.value*Mathf.Sin(severityY)*2 - severityY;

		Vector3 pp = cameraStartPosition;
		pp.x+= quakeAmtX;
		pp.y+= quakeAmtY;

		Camera.main.transform.position = pp;
	}

	public static void ReturnScreen(this Camera camera) {
		camera.transform.position = cameraStartPosition;
	}

	private static IEnumerator<float> TimedShake(this Camera camera, float x, float y) {
		Vector3 cameraStartPosition = camera.transform.position;
		float startTime = Time.time;
		float time = x + y;

		while (Time.time - time <= startTime) {
			Debug.Log (time + "  " + (Time.time - time) + "  " + startTime);
			float quakeAmtX = UnityEngine.Random.value * Mathf.Sin(x)*2 - x;
			float quakeAmtY = UnityEngine.Random.value*Mathf.Sin(y)*2 - y;

			Vector3 pp = cameraStartPosition;
			pp.x+= quakeAmtX;
			pp.y+= quakeAmtY;

			Camera.main.transform.position = pp;

			yield return 0f;
		}

		camera.transform.position = cameraStartPosition;
	}

	private static IEnumerator<float> Shake(this Camera camera, float x, float y) {
		Vector3 cameraStartPosition = camera.transform.position;
		float startTime = Time.time;
		float time = x + y;

		while (shake) {
			float quakeAmtX = UnityEngine.Random.value * Mathf.Sin(x)*2 - x;
			float quakeAmtY = UnityEngine.Random.value*Mathf.Sin(y)*2 - y;

			Vector3 pp = cameraStartPosition;
			pp.x+= quakeAmtX;
			pp.y+= quakeAmtY;

			Camera.main.transform.position = pp;

			yield return 0f;
		}
	}
}