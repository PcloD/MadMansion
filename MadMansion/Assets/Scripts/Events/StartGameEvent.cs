using UnityEngine;
using System.Collections;

public class StartGameEvent : GameEvent {
	public GameMode gameMode;
	public StartGameEvent (GameMode gameMode) {
		this.gameMode = gameMode;
	}
}
