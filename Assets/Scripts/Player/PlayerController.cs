using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private bool playerInputEnabled = true;
	public static Transform hero;
	private Vector2 input, inputPrev;

	private static GameObject slashOutline;
	private Vector3 slashOutlineScale;
	private Vector3 slashOutlineMaxScale = new Vector3(1.5f, 1.5f, 0);

	private LineRenderer slashLine, slashLineWrap, slashLineCollide;
	private SpriteRenderer spriteR;

	private SpriteAnimate animate;

	void Start()
	{
		hero = this.transform;
		slashOutline = transform.FindChild ("SlashOutline").gameObject;
		slashOutlineScale = slashOutline.transform.localScale;
		slashOutline.SetActive (false);

		spriteR = GetComponent<SpriteRenderer> ();

		slashLine = transform.FindChild ("SlashLine").GetComponent<LineRenderer>();
		slashLineWrap = transform.FindChild ("SlashLineWrap").GetComponent<LineRenderer>();
		slashLineCollide = transform.FindChild ("SlashLineCollide").GetComponent <LineRenderer>();

		animate = GetComponent <SpriteAnimate>();

		slashLine.enabled = false;
		slashLineWrap.enabled = false;
		slashLineCollide.enabled = false;
	}

	private static Vector3 newPosition;

	//## UPDATE ##//
	void Update () 
	{
		if (playerInputEnabled) 
		{
			inputPrev = input;
			input.y = InputWrapper.GetVerticalAxis ();
			input.x = InputWrapper.GetHorizontalAxis ();

			if (chargingSlash) ChargeSlash ();

			if (savedSlashCharge > 0) 
				ApplySlashDistance();
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
		if (input == Vector2.zero)
			return;

		float moveSpeed = chargingSlash ? walkSpeed : runSpeed;

		Vector3 newPosition = transform.position + (Vector3)input * moveSpeed * Time.deltaTime;

		// Smoothly rotate to face direction
		// TODO(samkern): Need to tune movement.
		if(Input.GetAxisRaw ("Horizontal") != 0 || Input.GetAxisRaw ("Vertical") != 0)
			Rotate((Vector3)input, false);
		
		transform.position = MoveRaycast(PlayerCamera.WrapWithinCameraBounds (newPosition));
	}

	private void Rotate(Vector3 dir, bool instant) {

		if (transform.eulerAngles == dir)
			return;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		if (instant)
			transform.rotation = q;
		else {
			transform.rotation = Quaternion.Slerp (transform.rotation, q, (Time.unscaledDeltaTime) * 8.0f);
			//if (Quaternion.Angle (transform.rotation, q) > 50.0f)
			//	AudioManager.PlayPlayerTurn ();
		}
	}

	private static RaycastHit2D hit;
	private bool alreadyColliding = false;
		
	private Vector3 MoveRaycast(Vector3 position)
	{
		// Player sprite length is about .5f, so we give it little more space.
		hit = Physics2D.Raycast (position, transform.up, .35f, 1 << LayerMask.NameToLayer("Impassable"));
		if(!hit)
			hit = Physics2D.Raycast (position, transform.up, .35f, 1 << LayerMask.NameToLayer("Debris"));

		if (hit) {
			if (!alreadyColliding) {
				AudioManager.PlayPlayerWallHit ();
				animate.Stretch (new Vector3(.6f, 1.2f, 0), .1f, true);
			}
			position = hit.point - ((Vector2)transform.up * .35f);
			alreadyColliding = true;
		} else {
			alreadyColliding = false;
		}

		return position;
	}

	private Vector3 SlashLinecast(Vector3 position, Vector3 newPosition)
	{
		if (slashLineCollide.enabled) {
			hit = Physics2D.Linecast (position, newPosition, 1 << LayerMask.NameToLayer ("Debris"));
			if (hit) {
				if (!alreadyColliding) {
					AudioManager.PlayPlayerWallHit ();
					animate.Stretch (new Vector3(.6f, 1.2f, 0), .1f, true);
				}
				KillSlashEarly ();
				newPosition = hit.point - ((Vector2)transform.up * .35f);
				alreadyColliding = true;
				return newPosition;
			}
		}
		
		hit = Physics2D.Linecast (position, newPosition, 1 << LayerMask.NameToLayer("Impassable"));
		if (hit) {
			if (!alreadyColliding) {
				AudioManager.PlayPlayerWallHit ();
				animate.Stretch (new Vector3(.6f, 1.2f, 0), .1f, true);
			}
			KillSlashEarly ();
			newPosition = hit.point - ((Vector2)transform.up * .35f);
			alreadyColliding = true;
		}

		return newPosition;
	}

	private bool chargingSlash = false;

	private float slashCharge;
	private float slashChargeRate = 4f;
	private float slashChargeMax = 6f;
	private float slashChargeBase = .2f;

	private float slashSpeed = 15f;

	private Vector3 slashDir;
	private float savedSlashCharge = 0.0f;

	private void BeginSlash() {
		Camera.main.LockCamera ();
		chargingSlash = true;
		slashCharge = slashChargeBase;
		slashLine.enabled = true;
		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, transform.position);

		slashOutline.SetActive (true);
	}

	private void ChargeSlash() {
		slashCharge = Mathf.Min(slashCharge + Time.unscaledDeltaTime * slashChargeRate, slashChargeMax);
		float bgFactor = (slashChargeMax - slashCharge)/slashChargeMax;
		Color color = new Color (1f, bgFactor, bgFactor, 1f);
		spriteR.color = color;

		if (slashOutline.activeSelf) {
			slashOutline.transform.localScale = slashOutlineScale + (slashOutlineMaxScale * bgFactor);
			if (bgFactor <= 0)
				slashOutline.SetActive (false);
		}

		// This is stupid just Lerp properly 
		Time.timeScale = Mathf.Clamp(Mathf.Lerp (0f,1.0f, bgFactor/2), 0.2f, 1.0f);
		Time.fixedDeltaTime = 0.02F * Time.timeScale;

		Camera.main.SingleShake (slashCharge/80, slashCharge/80);

		Vector3 slashVector = transform.position + slashCharge * transform.up;

		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, SlashLineLinecast(transform.position, slashVector));

		Vector3 debrisHit = SlashLineDebrisLinecast (transform.position, slashVector);
		if (debrisHit != Vector3.zero) {
			if (!slashLineCollide.enabled)
				slashLineCollide.enabled = true;
			slashLineCollide.SetPosition (0, transform.position);
			slashLineCollide.SetPosition (1, debrisHit);
		}
		else if(slashLineCollide.enabled) slashLineCollide.enabled = false;
	
		if (PlayerCamera.PositionOutsideBounds(slashVector)) {
			if(!slashLineWrap.enabled) slashLineWrap.enabled = true;

			slashLineWrap.SetPosition (0, PlayerCamera.WrapWithinCameraBounds (slashVector));
			slashLineWrap.SetPosition (1, PlayerCamera.WrapWithinCameraBounds (slashVector) + (slashVector - PlayerCamera.GetVectorToCameraBounds (slashVector)));
		} 
		else if(slashLineWrap.enabled) slashLineWrap.enabled = false;
	}

	private Vector3 SlashLineLinecast(Vector3 startLine, Vector3 endLine)
	{
		hit = Physics2D.Linecast (startLine, endLine, 1 << LayerMask.NameToLayer("Impassable"));
		if (hit)
			return hit.point;
		return endLine;
	}

	// returns Vector3.zero if we should NOT display the debris arrow.
	private Vector3 SlashLineDebrisLinecast(Vector3 startLine, Vector3 endLine)
	{
		hit = Physics2D.Linecast (startLine, endLine, 1 << LayerMask.NameToLayer("Debris"));
		if (hit && hit.collider.OverlapPoint((Vector2)endLine))
			return hit.point;
		return Vector3.zero;
	}

	private void ReleaseSlash() {
		AudioManager.PlayPlayerBoostRelease ();

		slashOutline.transform.localScale = slashOutlineScale;
		slashOutline.SetActive (false);

		chargingSlash = false;
		slashDir = transform.up;
		savedSlashCharge = slashCharge;

		if (slashLineCollide.enabled) slashLine.enabled = false;

		Camera.main.StartShake (slashCharge/30, slashCharge/30);
		slashCharge = 0.0f;
		gameObject.layer = LayerMask.NameToLayer ("SlashingHero");

		startSavedSlashCharge = savedSlashCharge;
		startTimeScale = Time.timeScale;
	}

	private float startSavedSlashCharge;
	private float startTimeScale;

	private void ApplySlashDistance () {
		Vector3 newPosition;
		Vector3 slashIncrement = slashDir * Time.unscaledDeltaTime * slashSpeed;
		if (savedSlashCharge <= slashIncrement.magnitude) {
			newPosition = PlayerCamera.WrapWithinCameraBounds (transform.position + savedSlashCharge * slashDir);
			savedSlashCharge = 0;
			slashLine.enabled = false;
			slashLineWrap.enabled = false;
			slashLineCollide.enabled = false;
			Camera.main.ReturnScreen ();
			gameObject.layer = LayerMask.NameToLayer ("Hero");
			transform.position = SlashLinecast(transform.position, newPosition);
			Time.timeScale = 1.0f;
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		} else {
			if (PlayerCamera.PositionOutsideBounds (transform.position + slashIncrement)) {
				slashLine.enabled = false;
				// Do I need to do a raycast here? ... probs not, right?
				transform.position = PlayerCamera.WrapWithinCameraBounds (transform.position + slashIncrement);
			} else
				transform.position = SlashLinecast(transform.position, transform.position + slashIncrement);

			if (slashLineCollide.enabled) 	slashLineCollide.SetPosition (0, transform.position);
			if (slashLine.enabled) 			slashLine.SetPosition (0, transform.position);
			else  							slashLineWrap.SetPosition (0, transform.position);

			savedSlashCharge -= slashIncrement.magnitude;
			Camera.main.SingleShake (savedSlashCharge/10, savedSlashCharge/10);
		}
		float bgFactor = (slashChargeMax - savedSlashCharge) / slashChargeMax;
		spriteR.color = new Color (1f, bgFactor, bgFactor, 1f);
		Time.timeScale = Mathf.Clamp(Mathf.Lerp (startTimeScale,1.0f,((startSavedSlashCharge - (savedSlashCharge/2))/startSavedSlashCharge)), 0.0f, 1.0f);
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
	}

	private void KillSlashEarly() {
		savedSlashCharge = 0;
		slashLine.enabled = false;
		slashLineWrap.enabled = false;
		slashLineCollide.enabled = false;
		Camera.main.ReturnScreen ();
		spriteR.color = Color.white;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
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
			Die ();
		}
		else if(LayerMask.LayerToName (collider.gameObject.layer) == "BlueDot") {
			if (collider.GetComponent<BlueDot> ().active) {
				animate.ColorFade (Color.white, new Color (104.0f / 255.0f, 155.0f / 255.0f, 1.0f), 0.2f, true);
				animate.Stretch (new Vector3 (1.6f, 1.6f, 0), .2f, true);
			}
		}
	}

	private void Die()
	{
		AudioManager.PlayPlayerDeath ();

		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		LevelMaster.Defeat();

		GameObject explosion = Instantiate (ParticleManager.playerExplosion);
		explosion.transform.position = transform.position;

		Destroy (this.gameObject);
	}
} 