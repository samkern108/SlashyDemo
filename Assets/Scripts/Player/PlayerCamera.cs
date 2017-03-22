using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public static Camera thisCamera;
	private static GameObject hero;

	public void Start() {
		// Hacky
		thisCamera = Camera.main;
		hero = GameObject.Find ("Hero");
	}

	public void Update() {
		if (hero) {
			Vector3 newPosition = hero.transform.position;
			newPosition.z = -10;
		}
		//this.transform.position = newPosition;
	}

	public static Vector3 WrapWithinCameraBounds(Vector3 newPosition) {
		if (newPosition.x < BoundsMin ().x)
			newPosition.x = BoundsMax ().x;
		else if (newPosition.x > BoundsMax ().x)
			newPosition.x = BoundsMin ().x;

		if (newPosition.y < BoundsMin ().y)
			newPosition.y = BoundsMax ().y;
		else if (newPosition.y > BoundsMax ().y)
			newPosition.y = BoundsMin ().y;

		return newPosition;
	}

	public static Vector2 BoundsMin()
	{
		return (Vector2)thisCamera.transform.position - Extents();
	}

	public static Vector2 BoundsMax()
	{
		return (Vector2)thisCamera.transform.position + Extents();
	}

	public static Vector2 Extents()
	{
		if (thisCamera.orthographic)
			return new Vector2(thisCamera.orthographicSize * Screen.width/Screen.height, thisCamera.orthographicSize);
		else
		{
			Debug.LogError("Camera is not orthographic!", thisCamera);
			return new Vector2();
		}
	}

	/*public SpriteRenderer background;

	private bool following = false;
	private Vector3 moveVector;
	private Transform target;

	private float followSpeed = 3f;

	private float max_x, min_x, max_y, min_y;

	void Start () {
		thisCamera = GetComponent<Camera>();

		Bounds b = background.bounds;
	
		float height = thisCamera.orthographicSize;
		float width = height * thisCamera.aspect;

		min_y = b.min.y + height;
		min_x = b.min.x + width;
		max_y = b.max.y - height;
		max_x = b.max.x - width;
	}

	private Vector3 newPosition;

	public void Update()
	{
		if (following) {
			moveVector = (target.position - transform.position) * followSpeed * Time.deltaTime;
			moveVector.z = 0;

			newPosition = transform.position + moveVector;
			newPosition.x = Mathf.Clamp (newPosition.x, min_x, max_x);
			newPosition.y = Mathf.Clamp (newPosition.y, min_y, max_y);
			transform.position = newPosition;
		}
	}

	private void SnapToTarget(Vector2 newPos)
	{
		transform.position = newPos;
	}*/
}
