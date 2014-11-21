using UnityEngine;
using System.Collections;

public enum Player {
	GhostPlayer,
	HunterPlayer,
	NoPlayer
}

public class EndGameEvent : GameEvent {
	private Player _winner;
	public Player Winner {
		get { return _winner; }
	}

	public EndGameEvent (Player winner) {
		_winner = winner;
	}
}
