using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private bool playerInputEnabled = true;
	public static Transform hero;
	private float hAxis, vAxis;

	private LineRenderer slashLine, slashLineWrap;
	private SpriteRenderer spriteR;

	void Start()
	{
		hero = this.transform;
		slashLine = transform.FindChild ("SlashLine").GetComponent<LineRenderer>();
		slashLineWrap = transform.FindChild ("SlashLineWrap").GetComponent<LineRenderer>();
		spriteR = GetComponent<SpriteRenderer> ();
		slashLine.enabled = false;
		slashLineWrap.enabled = false;
	}

	//## UPDATE ##//
	void Update () 
	{
		if (playerInputEnabled) 
		{
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();

			if (chargingSlash) ChargeSlash ();
				
			Linecasts ();

			if (savedSlashCharge > 0) ApplySlashDistance ();
			else {
				Move ();
				if (Input.GetKeyDown (KeyCode.Space)) 
					BeginSlash ();
				if (Input.GetKeyUp (KeyCode.Space) && chargingSlash) 
					ReleaseSlash ();
			}
		}
	}

	//## RUNNING ##//
	private float runSpeed = 4f, walkSpeed = 1f;

	private void Move() {
		if (vAxis == 0 && hAxis == 0)
			return;

		float moveSpeed = chargingSlash ? walkSpeed : runSpeed;

		Vector3 moveDir = new Vector3 (hAxis, vAxis);
		Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
		transform.position = PlayerCamera.WrapWithinCameraBounds (newPosition);

		// Smoothly rotate to face direction
		Rotate(moveDir, false);
	}

	private void Rotate(Vector3 dir, bool instant) {

		if (transform.eulerAngles == dir)
			return;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		if (instant)
			transform.rotation = q;
		else
			transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 8.0f);
	}

	// Maybe I can get LinecastNonAlloc to work someday.
	private void Linecasts()
	{
	}

	private bool chargingSlash = false;

	private float slashCharge;
	private float slashChargeRate = 10f;
	private float slashChargeMax = 10f;
	private float slashChargeBase = .2f;

	private float slashSpeed = 15f;

	private Vector3 slashDir;
	private float slashLength = 2.0f;
	private float savedSlashCharge = 0.0f;

	private void ChargeSlash() {
		slashCharge = Mathf.Min(slashCharge + Time.deltaTime * slashChargeRate, slashChargeMax);
		float bgFactor = (slashChargeMax - slashCharge)/slashChargeMax;
		Color color = new Color (1f, bgFactor, bgFactor, 1f);
		GetComponent<SpriteRenderer> ().color = color;

		Vector3 slashVector = transform.position + slashCharge * transform.up;

		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, slashVector);
	
		if (PlayerCamera.PositionOutsideBounds(slashVector)) {
			if(!slashLineWrap.enabled) slashLineWrap.enabled = true;

			slashLineWrap.SetPosition (0, PlayerCamera.WrapWithinCameraBounds (slashVector));
			slashLineWrap.SetPosition (1, PlayerCamera.WrapWithinCameraBounds (slashVector) + (slashVector - PlayerCamera.GetVectorToCameraBounds (slashVector)));
		} 
		else if(slashLineWrap.enabled) slashLineWrap.enabled = false;

		Time.timeScale = Mathf.Max(bgFactor * .16f, .4f);

		Camera.main.SingleShake (slashCharge/80, slashCharge/80);
	}

	private void BeginSlash() {
		Camera.main.LockCamera ();
		chargingSlash = true;
		slashCharge = slashChargeBase;
		slashLine.enabled = true;
		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, transform.position);
	}

	private void ReleaseSlash() {
		chargingSlash = false;
		slashDir = transform.up;
		savedSlashCharge = slashCharge;

		Camera.main.StartShake (slashCharge/30, slashCharge/30);
		slashCharge = 0.0f;
	}

	private void ApplySlashDistance() {
		Vector3 slashIncrement = slashDir * Time.deltaTime * slashSpeed;
		if (savedSlashCharge <= slashIncrement.magnitude) {
			transform.position = PlayerCamera.WrapWithinCameraBounds (transform.position + savedSlashCharge * slashDir);
			savedSlashCharge = 0;
			slashLine.enabled = false;
			slashLineWrap.enabled = false;
			Camera.main.ReturnScreen ();
			return;
		} else {
			if (PlayerCamera.PositionOutsideBounds (transform.position + slashIncrement)) {
				slashLine.enabled = false;
				transform.position = PlayerCamera.WrapWithinCameraBounds (transform.position + slashIncrement);
			} else {
				transform.position += slashIncrement;
			}
			if (slashLine.enabled) {
				slashLine.SetPosition (0, transform.position);
			} else {
				slashLineWrap.SetPosition (0, transform.position);
			}

			savedSlashCharge -= slashIncrement.magnitude;
			Camera.main.SingleShake (savedSlashCharge/10, savedSlashCharge/10);
		}
		float bgFactor = (slashChargeMax - savedSlashCharge) / slashChargeMax;
		spriteR.color = new Color (1f, bgFactor, bgFactor, 1f);
		Time.timeScale = Mathf.Min(bgFactor * 2, 1);

		//TODO Fix this later...
		/*RaycastHit2D[] hits = Physics2D.RaycastAll (transform.position, slashDir.normalized, slashLength);

		foreach(RaycastHit2D hit in hits) {
			Slashable slash = hit.collider.GetComponent <Slashable> ();
			if (slash)
				// We should pass the slash itself so enemies know how hard they got slashed.
				slash.Slashed ();
		}*/
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if (LayerMask.LayerToName (collision.gameObject.layer) == "Enemy") {
			if (savedSlashCharge > 0.0f) {
				collision.gameObject.GetComponent <Enemy> ().Slashed ();
			} else {
				Die ();
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (LayerMask.LayerToName (collider.gameObject.layer) == "Projectile") {
			if (savedSlashCharge <= 0.0f) {
				Die ();
			} else {
				StartCoroutine ("ColorFadeIn", Color.blue);
			}
		}
	}

	private void ColorFadeIn(Color color) {
		float startRG = 0.0f, endRG = 1.0f;
		float currentRG = startRG;
		while(currentRG <= endRG) {
			currentRG += Time.deltaTime;
			spriteR.color = Color.Lerp (Color.white, color, currentRG);
		}
		StartCoroutine ("ColorFadeOut", color);
	}

	private void ColorFadeOut(Color color) {
		float startRG = 1.0f, endRG = 0.0f;
		float currentRG = startRG;
		while(currentRG >= endRG) {
			currentRG -= Time.deltaTime;
			spriteR.color = Color.Lerp (Color.white, color, currentRG);
		}
	}

	private void Die()
	{
		Time.timeScale = 1;
		LevelMaster.Defeat();
		Destroy (this.gameObject);
	}
} 