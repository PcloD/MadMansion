using UnityEngine;
using System.Collections;

public class PossessionEvent : GameEvent {
	public bool succeeded;
	public PossessionEvent (bool succeeded) {
		this.succeeded = succeeded;
	}
}