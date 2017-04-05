using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

	public static GameObject projectileExplosion, playerExplosion;

	public static void Initialize () {
		projectileExplosion = ResourceLoader.LoadParticle ("Projectile Explode");
		playerExplosion = ResourceLoader.LoadParticle ("Player Explode");
	}
}
