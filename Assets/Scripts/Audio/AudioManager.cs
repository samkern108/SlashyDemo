﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioClip projectileShoot, projectileExplode, 
	playerBoostCharge, playerBoostRelease, playerWallHit, playerTurn, playerDeath,
	levelComplete, gameStart, 
	dotPickup;

	public static AudioSource dotAS, playerAS;

	public void Awake() {
		dotAS = transform.FindChild ("DotAS").GetComponent<AudioSource>();
		playerAS = transform.FindChild ("PlayerAS").GetComponent<AudioSource>();
	}

	public static void Initialize () {
		projectileShoot = ResourceLoader.LoadAudioClip ("Projectile Shoot");
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

	public static void PlayDotPickup() {
		dotAS.PlayOneShot (dotPickup);
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
}