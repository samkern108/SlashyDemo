using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDot : MonoBehaviour {

	public bool active = true;

	public List<BlueDot> activators = new List<BlueDot> ();
	private List<BlueDot> activates = new List<BlueDot> ();

	public void Start() {
		LevelMaster.AddBlueDot ();
		if (activators.Count > 0) {
			foreach(BlueDot activator in activators)
				activator.activates.Add (this);
			active = false;
		}
		GetComponent <Animation> ().enabled = active;
	}

	public void Activate(BlueDot activator) {
		activators.Remove (activator);
		if (activators.Count == 0) {
			active = true;
			GetComponent <Animation> ().enabled = active;
		}
	}
		
	public void OnTriggerEnter2D(Collider2D collider) {
		if (active) {
			AudioManager.instance.PlayDotPickup ();
			LevelMaster.CollectBlueDot ();
			foreach(BlueDot activated in activates)
				// TODO(samkern): Delay this activation and play a sound.
				activated.Activate (this);

			Destroy (this.gameObject);
		}
	}
}
