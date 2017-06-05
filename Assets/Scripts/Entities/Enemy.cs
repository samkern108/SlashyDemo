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

		InvokeRepeating ("Shoot", Random.Range(0.5f, 1.0f), shootTimer);
	}

	public override void Slashed() {
		Destroy (this.gameObject);
		LevelMaster.EnemyDied ();
	}

	private void Shoot() {
		if (!PlayerController.hero)
			return;
		
		Vector3 shootDirection = (PlayerController.hero.position - transform.position).normalized;

		/*RaycastHit2D hit = Physics2D.Linecast (transform.position, PlayerController.hero.position, 1 << LayerMask.NameToLayer("Impassable"));
		if (hit.collider) {
			return;
		}*/

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

	public void N_GameEnd(bool victory) {
		N_Pause (true);
	}

	public void N_Respawn() {
		N_Pause (false);
	}

	public void N_Pause(bool pause) {
		if (pause) {
			CancelInvoke ();
		} else {
			// TODO(samkern): Clean for exploits.
			InvokeRepeating ("Shoot", shootTimer, shootTimer);
		}
	}
}
