using UnityEngine;
using System.Collections;

public class HauntEvent : GameEvent {
	public float duration;
	private bool _started;
	public bool IsStart {
		get { return _started; }
	}
	public bool IsEnd {
		get { return !_started; }
	}

	public HauntEvent (bool starting, float duration) {
		_started = starting;
		this.duration = duration;
	}
}