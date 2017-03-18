using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public static Camera camera;
	private static GameObject hero;

	public void Start() {
		hero = GameObject.Find ("Hero");
	}

	public void Update() {
		if (hero) {
			Vector3 newPosition = hero.transform.position;
			newPosition.z = -10;
		}
		//this.transform.position = newPosition;
	}

	/*public SpriteRenderer background;

	private bool following = false;
	private Vector3 moveVector;
	private Transform target;

	private float followSpeed = 3f;

	private float max_x, min_x, max_y, min_y;

	void Start () {
		camera = GetComponent<Camera>();

		Bounds b = background.bounds;
	
		float height = camera.orthographicSize;
		float width = height * camera.aspect;

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
