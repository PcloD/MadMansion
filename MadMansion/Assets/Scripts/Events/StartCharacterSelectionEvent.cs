using UnityEngine;
using System.Collections;

public class StartCharacterSelectionEvent : GameEvent {

	public GameMode gameMode;
	public StartCharacterSelectionEvent (GameMode gameMode) {
		this.gameMode = gameMode;
	}

}
