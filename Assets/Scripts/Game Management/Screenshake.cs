using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public static class Screenshake {

	private static Vector3 cameraStartPosition;
	private static bool shake = false;
	private static Vector3 zoomFocal;
	private static int currentZoomInstance = 0;
	private static int zoomInstanceCounter = 0;

	public static void StartShake(this Camera camera, float severityX, float severityY, bool taperOff) {
		StartShake (camera, severityX, severityY, (severityX + severityY), taperOff);
	}

	public static void StartShake(this Camera camera, float severityX, float severityY, float time, bool taperOff) {
		shake = true;
		Timing.RunCoroutine (TimedShake(camera, severityX, severityY, time, taperOff));
	}

	public static void ZoomIn(this Camera camera, float rate, float amount) {
		Timing.RunCoroutine (ZoomCo(camera, rate, amount, PlayerCamera.originalPosition));	
	}

	public static void ZoomIn(this Camera camera, float time, float finalSize, Vector3 zoomFocal) {
		zoomFocal.z = PlayerCamera.originalPosition.z;
		Timing.RunCoroutine (ZoomCo(camera, time, finalSize, zoomFocal));	
	}

	public static void RestoreAll(this Camera camera, float time) {
		Timing.RunCoroutine (ZoomCo(camera, time, PlayerCamera.originalSize, PlayerCamera.originalPosition));
	}

	public static void RestoreSize(this Camera camera, float time) {
		Timing.RunCoroutine (ZoomCo(camera, time, PlayerCamera.originalSize, PlayerCamera.originalPosition));
	}

	// May be a useful concept someday?
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
		Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - .005f, PlayerCamera.originalSize - 1.0f);
	}

	public static void ReturnScreen(this Camera camera) {
		camera.transform.position = cameraStartPosition;
		camera.orthographicSize = PlayerCamera.originalSize;
	}

	private static IEnumerator<float> ZoomCo(this Camera camera, float time, float finalSize, Vector3 zoomFocal) {
		float startTime = Time.time;
		int instanceId = zoomInstanceCounter++;
		currentZoomInstance = instanceId;

		while((Time.time - time <= startTime) && (currentZoomInstance == instanceId)) {
			float timeFactor = (Time.time - startTime) / time;
			camera.orthographicSize = Mathf.Lerp (PlayerCamera.originalSize, finalSize, timeFactor);
			camera.transform.position = Vector3.Lerp (camera.transform.position, zoomFocal, timeFactor);
			yield return 0f;
		}
		//camera.orthographicSize = finalSize;	
		//camera.transform.position = zoomFocal;
	}

	private static IEnumerator<float> TimedShake(this Camera camera, float x, float y, float time, bool taperOff) {
		Vector3 cameraStartPosition = camera.transform.position;
		float startTime = Time.time;
		float startX = x;
		float startY = y;

		while (Time.time - time <= startTime) {
			float quakeAmtX = UnityEngine.Random.value * Mathf.Sin(x) * 2 - x;
			float quakeAmtY = UnityEngine.Random.value *  Mathf.Sin(y) * 2 - y;

			Vector3 pp = cameraStartPosition;
			pp.x+= quakeAmtX;
			pp.y+= quakeAmtY;

			if (taperOff) {
				x = Mathf.Lerp (startX, 0, (Time.time - startTime)/time);
				y = Mathf.Lerp (startY, 0, (Time.time - startTime)/time);
			}

			Camera.main.transform.position = pp;

			yield return 0f;
		}
	}
}