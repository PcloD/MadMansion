using UnityEngine;
using System.Collections;

public enum Player {
	GhostPlayer,
	HunterPlayer,
	NoPlayer
}

public enum EndReason {
	HunterCaughtGhost,
	GhostHauntedHouse,
	HunterCaughtGhostInSameBody,
	HunterCaughtInnocent
}

public class EndGameEvent : GameEvent {
	public Player winner;
	public EndReason rationale;

	public EndGameEvent (Player winner, EndReason rationale) {
		this.winner = winner;
		this.rationale = rationale;
	}
}
