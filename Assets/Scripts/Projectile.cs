using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private Vector3 moveDir;
	private float moveSpeed = 1f;

	public void Initialize (Vector2 moveDir) {
		this.moveDir = moveDir;
	}
	
	void Update () {
		transform.position += moveDir * moveSpeed * Time.deltaTime;
	}

	// Destroy the missile when no camera can see it any longer.
	public void OnBecameInvisible() {
		Destroy (this.gameObject);
	}

	public void GameOver() {
		moveSpeed = 0f;
	}
}
