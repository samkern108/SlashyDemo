using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private bool playerInputEnabled = true;
	public static Transform hero;
	private float hAxis, vAxis;

	private LineRenderer slashLine;
	private SpriteRenderer spriteR;

	void Start()
	{
		hero = this.transform;
		slashLine = GetComponentInChildren <LineRenderer>();
		spriteR = GetComponent<SpriteRenderer> ();
		slashLine.enabled = false;
	}

	//## UPDATE ##//
	void Update () 
	{
		if (playerInputEnabled) 
		{
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();

			if (chargingSlash) {
				ChargeSlash ();
			}

			if(savedSlashCharge > 0) {
				ApplySlashDistance ();
			}
			
			//1: Conduct Linecasts
			Linecasts ();
			Move ();
			if (Input.GetKeyDown (KeyCode.Space))
				BeginSlash ();
			if (Input.GetKeyUp (KeyCode.Space) && chargingSlash)
				ReleaseSlash ();
		}
	}

	//## RUNNING ##//
	private float runSpeed = 4f, walkSpeed = 1f;

	private void Move() {
		if (vAxis == 0 && hAxis == 0)
			return;

		/*float angle = Mathf.Atan2(vAxis, hAxis) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/

		float moveSpeed = chargingSlash ? walkSpeed : runSpeed;

		transform.position += new Vector3 (hAxis, vAxis) * moveSpeed * Time.deltaTime;
	}

	// Maybe I can get LinecastNonAlloc to work someday.
	private void Linecasts()
	{
	}

	private float slashCharge;
	private float slashChargeRate = 4f;
	private float slashChargeMax = 10f;
	private float slashChargeBase = .2f;
	private bool chargingSlash = false;

	private void ChargeSlash() {
		slashCharge = Mathf.Min(slashCharge + Time.deltaTime * slashChargeRate, slashChargeMax);
		float bgFactor = (slashChargeMax - slashCharge)/slashChargeMax;
		Color color = new Color (1f, bgFactor, bgFactor, 1f);
		GetComponent<SpriteRenderer> ().color = color;
		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition(1, transform.position + slashCharge * new Vector3(hAxis, vAxis, 0));
	}

	private void BeginSlash() {
		chargingSlash = true;
		slashCharge = slashChargeBase;
		slashLine.enabled = true;
		slashLine.SetPosition (0, transform.position);
		slashLine.SetPosition (1, transform.position);
	}

	private void ReleaseSlash() {
		chargingSlash = false;
		slashDir = new Vector3 (hAxis, vAxis);
		savedSlashCharge = slashCharge;

		slashCharge = 0.0f;
	}

	private Vector3 slashDir;
	private float slashSpeed = 6f;

	private float slashLength = 2.0f;
	private float savedSlashCharge = 0.0f;

	private void ApplySlashDistance() {
		Vector3 slashIncrement = slashDir * Time.deltaTime * slashSpeed;
		if (savedSlashCharge <= slashIncrement.magnitude) {
			transform.position += savedSlashCharge * slashDir;
			savedSlashCharge = 0;
			slashLine.enabled = false;
		} else {
			transform.position += slashIncrement;
			savedSlashCharge -= slashIncrement.magnitude;
		}
		float bgFactor = (slashChargeMax - savedSlashCharge) / slashChargeMax;
		spriteR.color = new Color (1f, bgFactor, bgFactor, 1f);

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
				Destroy (this.gameObject);
			}
		}
		else if (LayerMask.LayerToName (collision.gameObject.layer) == "Projectile") {
			if (savedSlashCharge <= 0.0f) {
				Destroy (this.gameObject);
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
} 