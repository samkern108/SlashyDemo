using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Slashable {

	private static GameObject missilePrefab;
	private float shootTimer = 0.5f;

	void Start () {
		LevelMaster.enemiesRemaining++;

		if (!missilePrefab)
			missilePrefab = ResourceLoader.LoadPrefab ("Projectile");

		InvokeRepeating ("Shoot", shootTimer, shootTimer);
	}

	public override void Slashed() {
		Destroy (this.gameObject);
		LevelMaster.EnemyDied ();
	}

	private void Shoot() {
		GameObject missile = Instantiate (missilePrefab, transform.parent);
		missile.transform.position = this.transform.position;
		missile.GetComponent<Projectile>().Initialize (PlayerController.hero.position - transform.position);
	}

	// Notifications

	public void GameOver() {
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
