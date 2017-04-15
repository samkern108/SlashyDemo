using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	public static AudioClip projectileShoot, projectileExplode, 
	playerBoostCharge, playerBoostRelease, playerWallHit, playerTurn, playerDeath,
	levelComplete, gameStart, 
	dotPickup;

	private static GameObject sourceTemplate;
	private static int activeSources = 0;
	private static List<AudioSource> freeSources = new List<AudioSource>();

	public static AudioSource dotAS, playerAS;

	public void Awake() {
		instance = this;
		sourceTemplate = new GameObject("SourceTemplate");
		sourceTemplate.AddComponent <AudioSource>();
		sourceTemplate.transform.SetParent (this.transform);

		dotAS = transform.FindChild ("DotAS").GetComponent<AudioSource>();
		playerAS = transform.FindChild ("PlayerAS").GetComponent<AudioSource>();
	}

	public static void Initialize () {
		projectileShoot = ResourceLoader.LoadAudioClip ("Shoot");
		projectileExplode = ResourceLoader.LoadAudioClip ("Projectile Explode");
		playerBoostCharge = ResourceLoader.LoadAudioClip ("Player Boost Charge");
		playerBoostRelease = ResourceLoader.LoadAudioClip ("Player Boost Release");
		playerWallHit = ResourceLoader.LoadAudioClip ("Player Wall Hit");
		playerTurn = ResourceLoader.LoadAudioClip ("Player Turn");
		playerDeath = ResourceLoader.LoadAudioClip ("Player Death");
		levelComplete = ResourceLoader.LoadAudioClip ("Level Complete");
		gameStart = ResourceLoader.LoadAudioClip ("Game Start");
		dotPickup = ResourceLoader.LoadAudioClip ("Dot Pickup");
	}

	private static bool resetPickup = true;
		
	public void PlayDotPickup() {
		if (resetPickup) {
			resetPickup = false;
			dotAS.pitch = .55f;
		}

		dotAS.PlayOneShot (dotPickup);
		dotAS.pitch += .25f;
	}
		
	public void LevelLoaded(int level) {
		resetPickup = true;
	}

	public static void PlayPlayerDeath() {
		playerAS.PlayOneShot (playerDeath);
	}

	public static void PlayPlayerTurn() {
		playerAS.PlayOneShot (playerTurn);
	}

	public static void PlayPlayerBoostCharge() {
		playerAS.PlayOneShot (playerBoostCharge);
	}

	public static void PlayPlayerBoostRelease() {
		playerAS.PlayOneShot (playerBoostRelease);
	}

	public static void PlayPlayerWallHit() {
		playerAS.PlayOneShot (playerTurn);
	}

	public static void PlayShoot() {
		AudioSource source;
		if (freeSources.Count == 0) {
			GameObject obj = Instantiate (sourceTemplate);
			obj.transform.SetParent (AudioManager.instance.transform);
			source = obj.GetComponent <AudioSource>();
			activeSources++;
		}
		else {
			// Race condition?
			source = freeSources[0];
			freeSources.RemoveAt (0);
			// Figure out a better way to determine if sources can be removed.
			if (freeSources.Count >= (int)Mathf.Ceil (activeSources / 2)) {
				freeSources.RemoveRange (0, (int)Mathf.Floor (activeSources / 2));
			}
		}
		source.PlayOneShot (projectileShoot);
		source.pitch = Random.Range (.8f, 1.2f);
		float wait = projectileShoot.length;
		Timing.RunCoroutine (RecycleSource(wait, source));
	}

	public static void PlayProjectileExplode() {
		
	}

	private static IEnumerator<float> RecycleSource (float wait, AudioSource source) {
		yield return wait;
		//source.pitch = 1.0f;
		freeSources.Add (source);
	}
}
