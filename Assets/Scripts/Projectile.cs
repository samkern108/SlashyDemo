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
}
