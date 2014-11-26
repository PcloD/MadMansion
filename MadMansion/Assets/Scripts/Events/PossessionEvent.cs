using UnityEngine;
using System.Collections;

public class PossessionEvent : GameEvent {
	public bool succeeded;
	public Room room;
	public PossessionEvent (bool succeeded, Room room) {
		this.succeeded = succeeded;
		this.room = room;
	}
}