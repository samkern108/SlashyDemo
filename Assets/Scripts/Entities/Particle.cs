using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

	void Start () {
		Invoke ("KillSelf", GetComponent<ParticleSystem>().main.duration);
	}
	
	void KillSelf () {
		Destroy (gameObject);
	}
}
