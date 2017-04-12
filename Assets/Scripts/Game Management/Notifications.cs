using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifications : MonoBehaviour {

	public static Notifications self;

	public void Awake() {
		self = this;
	}

	public void SendGameEndNotification(bool victory) {
		BroadcastMessage ("GameEnd", victory);
	}

	public void SendPauseNotification(bool pause) {
		BroadcastMessage ("Pause",pause);
	}

	public void SendRestartNotification() {
		BroadcastMessage ("Restart");
	}

	public void SendLoadLevelNotification(int level) {
		BroadcastMessage ("LevelLoaded", level);
	}
}
