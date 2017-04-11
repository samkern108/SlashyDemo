using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private Vector3 moveDir;
	private float moveSpeed = 5f;

	public void Initialize (Vector2 moveDir) {
		this.moveDir = moveDir;
	}
	
	void Update () {
		transform.position += moveDir * moveSpeed * Time.deltaTime;
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (LayerMask.LayerToName (collider.gameObject.layer) == "Impassable")
			Explode ();
	}

	private void Explode() {
		GameObject explosion = Instantiate (ParticleManager.projectileExplosion);
		explosion.transform.position = transform.position - (moveDir * GetComponent<PolygonCollider2D>().bounds.size.x);

		float angle = Mathf.Atan2 (moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90;
		explosion.transform.RotateAround (transform.position, -Vector3.forward, angle);
		
		Destroy (this.gameObject);
	}

	// Destroy the missile when no camera can see it any longer.
	public void OnBecameInvisible() {
		Destroy (this.gameObject);
	}

	public void GameEnd(bool victory) {
		moveSpeed = 0f;
	}
}
