using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Slashable {

	private SpriteAnimate animate;
	private static GameObject missilePrefab;
	private float shootTimer = 0.5f;

	void Start () {
		animate = GetComponent <SpriteAnimate>();
		animate.Stretch (new Vector3(0,0,0),1.0f,false,false);
		LevelMaster.enemiesRemaining++;

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

		animate.Stretch (new Vector3(1.2f, 1.2f, 1.2f), .2f, true, true);
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
