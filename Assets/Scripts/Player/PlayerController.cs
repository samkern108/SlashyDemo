using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Vector2 input, inputPrev;
	private bool playerInputEnabled = true;

	public static Transform hero;
	private SpriteRenderer spriteR;
	private SpriteAnimate animate;
	private LineRenderer slashLine, slashLineWrap;
	private ParticleSystem sprayPS, dashPS;

	private static GameObject slashOutline;
	private Vector3 slashOutlineScale;
	private Vector3 slashOutlineMaxScale = new Vector3(1.5f, 1.5f, 0);

	private float runSpeed = 5.5f, walkSpeed = 3f;
	private float currentSpeed = 0f;

	private Vector3 originalScale;

	private static Vector3 newPosition;

	void Start()
	{
		hero = this.transform;
		originalScale = transform.localScale;
		slashOutline = transform.FindChild ("SlashOutline").gameObject;
		slashOutlineScale = slashOutline.transform.localScale;
		slashOutline.SetActive (false);

		dashPS = transform.FindChild ("Dash").GetComponent<ParticleSystem>();
		dashPS.Stop ();

		sprayPS = transform.FindChild ("Spray").GetComponent<ParticleSystem>();
		spriteR = GetComponent<SpriteRenderer> ();

		slashLine = transform.FindChild ("SlashLine").GetComponent<LineRenderer>();
		slashLineWrap = transform.FindChild ("SlashLineWrap").GetComponent<LineRenderer>();

		animate = GetComponent <SpriteAnimate>();

		slashLine.enabled = false;
		slashLineWrap.enabled = false;
	}

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

	private void Move() {
		if (input == Vector2.zero) {
			currentSpeed = 0;
			return;
		}
			
		// Tune coefficients to control ramp up speed
		currentSpeed = ((1 * currentSpeed) + (chargingSlash ? walkSpeed : runSpeed))/2;
		//currentSpeed = Mathf.Clamp (currentSpeed + .3f, 0f, (chargingSlash ? walkSpeed : runSpeed));

		Vector3 newPosition = transform.position + (Vector3)input * currentSpeed * Time.deltaTime;

		// Smoothly rotate to face direction
		// TODO(samkern): Need to tune movement.
		if(Input.GetAxisRaw ("Horizontal") != 0 || Input.GetAxisRaw ("Vertical") != 0)
			Rotate((Vector3)input, false);
		else
			transform.localScale = originalScale;
		
		transform.position = MoveRaycast(PlayerCamera.WrapWithinCameraBounds (newPosition));
	}

	private void Rotate(Vector3 dir, bool instant) {

		if (transform.eulerAngles == dir) return;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

		if (instant) transform.rotation = q;
		else {
			transform.rotation = Quaternion.Slerp (transform.rotation, q, (Time.unscaledDeltaTime) * 8.0f);
			float rotAngle = Quaternion.Angle (transform.rotation, q);
			currentSpeed = Mathf.Clamp(currentSpeed - (currentSpeed * (rotAngle / 1080)), 0, currentSpeed);
			Vector3 scale = originalScale;
			scale.x *= (1 - rotAngle / 360);
			scale.y *= (1 + rotAngle / 360);
			transform.localScale = scale;
			if (rotAngle > 90) {
				AudioManager.PlayPlayerTurn ();
			}
			//if (Quaternion.Angle (transform.rotation, q) > 50.0f)
			//	AudioManager.PlayPlayerTurn ();
		}
	}

	private static RaycastHit2D hit;
	private bool alreadyColliding = false;
		
	private Vector3 MoveRaycast(Vector3 position)
	{
		// Player sprite length is about .5f, so we give it little more space.
		hit = Physics2D.Raycast (position, transform.up, .30f, 1 << LayerMask.NameToLayer("Impassable"));
		if(!hit)
			hit = Physics2D.Raycast (position, transform.up, .30f, 1 << LayerMask.NameToLayer("Debris"));

		if (hit) {
			if (!alreadyColliding) {
				AudioManager.PlayPlayerWallHit ();
				animate.Stretch (new Vector3(1.2f, .6f, 0), .1f, true, true);
			}
			position = hit.point - ((Vector2)transform.up * .30f);
			alreadyColliding = true;
			currentSpeed = 0;
		} else
			alreadyColliding = false;

		return position;
	}

	private bool chargingSlash = false;

	private float slashCharge;
	private float slashChargeRate = 4f;
	private float slashChargeMax = 6f;
	private float slashChargeBase = .2f;

	private float slashSpeed = 15f;

	private Vector3 slashDir;
	private float savedSlashCharge = 0.0f;

	private float startSavedSlashCharge;
	private float startTimeScale;

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
		spriteR.color = new Color (1f, bgFactor, bgFactor, 1f);

		if (slashOutline.activeSelf) {
			slashOutline.transform.localScale = slashOutlineScale + (slashOutlineMaxScale * bgFactor);
			if (bgFactor <= 0)
				slashOutline.SetActive (false);
		}

		// This is stupid just lerp properly 
		Time.timeScale = Mathf.Clamp(Mathf.Lerp (0f,1.0f, bgFactor/2), 0.2f, 1.0f);
		Time.fixedDeltaTime = 0.02F * Time.timeScale;

		Camera.main.SingleShake (slashCharge/80, slashCharge/80);

		Vector3 slashVector = transform.position + slashCharge * transform.up;

		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, SlashLineLinecast(transform.position, slashVector));

		Vector3 debrisHit = SlashLineDebrisLinecast (transform.position, slashVector);
		if (debrisHit != Vector3.zero) {
			slashLine.colorGradient = Palette.slashLineRed;
			slashThroughDebris = false;
		} else {
			slashLine.colorGradient = Palette.slashLineYellow;
			slashThroughDebris = true;
		}

	
		if (PlayerCamera.PositionOutsideBounds(slashVector)) {
			if(!slashLineWrap.enabled) slashLineWrap.enabled = true;

			slashLineWrap.SetPosition (0, PlayerCamera.GetVectorToCameraBoundsWrapped (transform.position, slashVector));
			slashLineWrap.SetPosition (1, PlayerCamera.WrapWithinCameraBounds (slashVector) + (slashVector - PlayerCamera.GetVectorToCameraBounds (slashVector)));
		} 
		else if(slashLineWrap.enabled) slashLineWrap.enabled = false;
	}

	private void ReleaseSlash() {
		AudioManager.PlayPlayerBoostRelease (slashCharge.Map(0, slashChargeMax, 1.2f, .8f), slashCharge.Map(0, slashChargeMax, .5f, 1f));
		//dashPS.Play ();

		slashOutline.transform.localScale = slashOutlineScale;
		slashOutline.SetActive (false);

		chargingSlash = false;
		slashDir = transform.up;
		savedSlashCharge = slashCharge;

		Camera.main.StartShake (slashCharge/15, slashCharge/15, true);
		Camera.main.RestoreSize (1.0f);
		slashCharge = 0.0f;
		gameObject.layer = LayerMask.NameToLayer ("SlashingHero");

		startSavedSlashCharge = savedSlashCharge;
		startTimeScale = Time.timeScale;
	}

	private void ApplySlashDistance () {
		Vector3 newPosition;
		Vector3 slashIncrement = slashDir * Time.unscaledDeltaTime * slashSpeed;
		if (savedSlashCharge <= slashIncrement.magnitude) {
			newPosition = PlayerCamera.WrapWithinCameraBounds (transform.position + savedSlashCharge * slashDir);
			transform.position = SlashLinecast (transform.position, newPosition);
			EndSlash ();
			return;
		} else {
			if (PlayerCamera.PositionOutsideBounds (transform.position + slashIncrement)) {
				slashLine.enabled = false;
				// Do I need to do a raycast here? ... probs not, right?
				transform.position = PlayerCamera.WrapWithinCameraBounds (transform.position + slashIncrement);
			} else
				transform.position = SlashLinecast (transform.position, transform.position + slashIncrement);

			SlashingThroughWater ();
			
			if (slashLine.enabled)
				slashLine.SetPosition (0, transform.position);
			else
				slashLineWrap.SetPosition (0, transform.position);

			savedSlashCharge -= slashIncrement.magnitude;
			Camera.main.SingleShake (savedSlashCharge / 10, savedSlashCharge / 10);

			float bgFactor = (slashChargeMax - savedSlashCharge) / slashChargeMax;
			spriteR.color = new Color (1f, bgFactor, bgFactor, 1f);
			Time.timeScale = Mathf.Clamp (Mathf.Lerp (startTimeScale, 1.0f, ((startSavedSlashCharge - (savedSlashCharge / 2)) / startSavedSlashCharge)), 0.0f, 1.0f);
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}
	}

	private bool slashThroughDebris = false;

	// Determine if we should run into a wall or into debris
	private Vector3 SlashLinecast(Vector3 position, Vector3 newPosition)
	{
		if (!slashThroughDebris)
			hit = Physics2D.Linecast (position, newPosition, 1 << LayerMask.NameToLayer ("Debris"));
		else
			hit = Physics2D.Linecast (position, newPosition, 1 << LayerMask.NameToLayer ("Impassable"));

		if (hit) {
			AudioManager.PlayPlayerWallHit ();
			animate.Stretch (new Vector3 (2.0f, 0.2f, 0), .1f, true, true);
			EndSlash ();
			newPosition = hit.point - ((Vector2)transform.up * .30f);
			alreadyColliding = true;
		}
		return newPosition;
	}

	private Vector3 SlashLineLinecast(Vector3 startLine, Vector3 endLine)
	{
		hit = Physics2D.Linecast (startLine, endLine, 1 << LayerMask.NameToLayer("Impassable"));
		//return hit ? hit.point : endLine;
		if (hit) return hit.point;
		return endLine;
	}

	// TODO(samkern): Should return the raycast hit instead.
	// returns Vector3.zero if we should NOT display the debris arrow.
	private Vector3 SlashLineDebrisLinecast(Vector3 startLine, Vector3 endLine)
	{
		hit = Physics2D.Linecast (startLine, endLine, 1 << LayerMask.NameToLayer("Debris"));
		if (hit && hit.collider.OverlapPoint((Vector2)endLine))
			return hit.point;
		return Vector3.zero;
	}

	private void SlashingThroughWater() {
		//hit = Physics2D.Raycast (transform.position, Vector2.up, .1f, 1 << LayerMask.NameToLayer ("Debris"));
		//if(hit) sprayPS.Emit (2);
		sprayPS.Emit (2);
	}

	private void EndSlash() {
		gameObject.layer = LayerMask.NameToLayer ("Hero");
		savedSlashCharge = 0;
		slashLine.enabled = false;
		slashLineWrap.enabled = false;
		Camera.main.ReturnScreen ();
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		spriteR.color = Color.white;
		//dashPS.Stop ();
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if (LayerMask.LayerToName (collision.gameObject.layer) == "Enemy") {
			if (savedSlashCharge > 0.0f)
				collision.gameObject.GetComponent <Enemy> ().Slashed ();
			else
				Die ();
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (LayerMask.LayerToName (collider.gameObject.layer) == "Projectile")
			Die ();
		else if(LayerMask.LayerToName (collider.gameObject.layer) == "BlueDot") {
			if (collider.GetComponent<BlueDot> ().active) {
				animate.ColorFade (Color.white, new Color (.4f, .74f, .93f), 0.2f, true);
				animate.Stretch (new Vector3 (1.6f, 1.6f, 0), .2f, true, true);
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

		Camera.main.StartShake (.3f, .3f, 2f, true);
		//Camera.main.ZoomIn (10.0f, PlayerCamera.originalSize - 2.0f, transform.position);

		Destroy (this.gameObject);
	}

	public void N_GameEnd(bool victory) {
		playerInputEnabled = false;
	}

	public void N_Restart() {
		playerInputEnabled = true;
		//Camera.main.RestoreAll (1.0f);
	}
} 