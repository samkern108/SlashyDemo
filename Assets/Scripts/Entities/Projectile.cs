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
		// Make sure we're including all things projectiles can hit
		RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir,2f, 1 << LayerMask.NameToLayer("Impassable"));
		if (hit.collider) {
			GameObject explosion = Instantiate (ParticleManager.projectileExplosion);
			explosion.transform.position = transform.position - (moveDir * 2 * GetComponent<PolygonCollider2D>().bounds.size.x/3);

			Vector2 reflection = Vector2.Reflect (moveDir, hit.normal);
			explosion.transform.rotation = Quaternion.LookRotation(reflection,Vector3.up);

			AudioManager.PlayProjectileExplode ();
		}
		
		Destroy (this.gameObject);
	}

	// Destroy the missile when no camera can see it any longer.
	public void OnBecameInvisible() {
		Destroy (this.gameObject);
	}

	public void GameEnd(bool victory) {
		moveSpeed = 0f;
	}

	public void Respawn() {
		Destroy (this.gameObject);
	}
}
