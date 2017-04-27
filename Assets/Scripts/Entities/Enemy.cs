using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Slashable {

	private Animator animator;
	private static GameObject missilePrefab;
	public float shootTimer = 0.4f;

	void Start () {
		animator = GetComponent <Animator>();
		animator.SetTrigger ("Spawn");

		if (!missilePrefab)
			missilePrefab = ResourceLoader.LoadPrefab ("Projectile");

		InvokeRepeating ("Shoot", Random.Range(1.0f, 2.0f), shootTimer);
	}

	public override void Slashed() {
		Destroy (this.gameObject);
		LevelMaster.EnemyDied ();
	}

	private void Shoot() {
		Vector3 shootDirection = (PlayerController.hero.position - transform.position).normalized;
		float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = q;

		GameObject missile = Instantiate (missilePrefab, transform.parent);
		missile.transform.position = this.transform.position;
		missile.GetComponent<Projectile>().Initialize (shootDirection);

		animator.SetTrigger ("Shoot");
		AudioManager.PlayShoot ();
	}

	// Notifications

	public void GameEnd() {
		CancelInvoke ();
	}

	public void Pause(bool pause) {
		if (pause) {
			CancelInvoke ();
		} else {
			// TODO(samkern): Clean for exploits.
			InvokeRepeating ("Shoot", shootTimer, shootTimer);
		}
	}
}
