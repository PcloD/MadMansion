using UnityEngine;
using System.Collections;

public class CatchEvent : GameEvent {
	public bool successful;
	public CatchEvent (bool successful) {
		this.successful = successful;
	}
}
