using UnityEngine;
using System.Collections;

public class HauntEvent : GameEvent {
	public float duration;
	public bool succeeded;
	public Room room;
	private bool _started;
	public bool IsStart {
		get { return _started; }
	}
	public bool IsEnd {
		get { return !_started; }
	}

	public HauntEvent (bool succeeded, bool starting, float duration, Room room) {
		_started = starting;
		this.duration = duration;
		this.room = room;
		this.succeeded = succeeded;
	}
}