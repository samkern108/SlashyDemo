using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDot : MonoBehaviour {

	private bool active = true;

	public BlueDot activator;
	private List<BlueDot> activates = new List<BlueDot> ();

	public void Start() {
		LevelMaster.AddBlueDot ();
		if (activator) {
			activator.activates.Add (this);
			active = false;
		}
		GetComponent <Animation> ().enabled = active;
	}

	public void Activate() {
		active = true;
		GetComponent <Animation> ().enabled = active;
	}
		
	public void OnTriggerEnter2D(Collider2D collider) {
		if (active) {
			LevelMaster.CollectBlueDot ();
			foreach(BlueDot activated in activates)
				activated.Activate ();

			Destroy (this.gameObject);
		}
	}
}
