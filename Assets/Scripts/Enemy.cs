using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Slashable {

	private static GameObject missilePrefab;
	private float shootTimer = 0.5f;

	void Start () {
		if (!missilePrefab)
			missilePrefab = ResourceLoader.LoadPrefab ("Projectile");

		InvokeRepeating ("Shoot", shootTimer, shootTimer);
	}

	public override void Slashed() {
		Destroy (this.gameObject);
	}

	private void Shoot() {
		GameObject missile = Instantiate (missilePrefab);
		missile.transform.position = this.transform.position;
		missile.GetComponent<Projectile>().Initialize (PlayerController.hero.position - transform.position);
	}
}
