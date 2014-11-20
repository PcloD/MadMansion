using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostTracker : MonoBehaviour {

	public static GhostTracker g;

	private Vector3[] _pastLocations;
	private int _maxCount = 0;
	private int _currCount;
	private float _timeDelay = 5f;

	private bool _canSeeHistory = false;
	public bool CanSeeHistory {
		get { return _canSeeHistory; }
	}

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	void Start () {
		float dt = Time.fixedDeltaTime;
		Debug.Log("DT: "+dt);
		_maxCount = (int)Mathf.Ceil(_timeDelay/dt);
		_pastLocations = new Vector3[_maxCount];
	}

	public void RecordLocation (Vector3 loc) {
		_pastLocations[_currCount] = loc;
		_currCount ++;
		if (_currCount >= _maxCount) {
			_canSeeHistory = true;
			_currCount = 0;
		}
	}

	public Vector3 HistoricalLocation {
		get {
			int sampleIndex = (_maxCount - _currCount - 1);
			return _pastLocations[sampleIndex];
		}
	}
}