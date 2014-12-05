using UnityEngine;
using System.Collections;

public class SmellEvent : GameEvent {
	private bool starting;
	public bool IsStart {
		get { return starting; }
	}
	public bool IsEnd {
		get { return !starting; }
	}

	public HunterController hunter;
	public Room room;

	public SmellEvent (bool starting, Room room = null, HunterController hunter = null) {
		this.starting = starting;
		this.hunter = hunter;
		this.room = room;
	}
}