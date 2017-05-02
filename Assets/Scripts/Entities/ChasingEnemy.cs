using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : Slashable {

	private SpriteAnimate animate;
	private float moveSpeed = 3f;
	private bool moving = true;

	void Start () {
		animate = GetComponent <SpriteAnimate>();
		animate.Stretch (new Vector3(0,0,0),1.0f,false,false);
		LevelMaster.enemiesRemaining++;
	}

	public override void Slashed() {
		Destroy (this.gameObject);
		LevelMaster.EnemyDied ();
	}

	private void Update() {
		if (moving) {
			Vector3 moveDirection = (PlayerController.hero.position - transform.position).normalized;
			float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			transform.position += moveDirection * moveSpeed * Time.deltaTime;
			transform.rotation = q;
		}
	}

	// Notifications

	public void N_Pause(bool pause) {
		moving = !pause;
	}
}
