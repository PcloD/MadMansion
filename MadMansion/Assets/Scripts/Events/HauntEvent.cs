using UnityEngine;
using System.Collections;

public class HauntEvent : GameEvent {
	private bool _started;
	public bool IsStart {
		get { return _started; }
	}
	public bool IsEnd {
		get { return !_started; }
	}

	public HauntEvent (bool starting) {
		_started = starting;
	}
}