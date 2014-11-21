using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HauntManager : MonoBehaviour {

	public static HauntManager g;
	[SerializeField]
	private float _hauntChargeDuration = 4f;
	[SerializeField]
	private float _hauntDuration = 4f;
	public float HauntTimerPercentage {
		get {
			return Mathf.Min(1f, _hauntProgressTimer.ElapsedMilliseconds/(_hauntDuration * 1000f));
		}
	}
	public float HauntChargePercentage {
		get {
			return Mathf.Min(1f, _hauntChargeTimer.ElapsedMilliseconds/(_hauntChargeDuration * 1000f));
		}
	}
	[SerializeField]
	private int _requiredHauntCount = 4;
	public int RequiredHauntCount {
		get { return _requiredHauntCount; }
	}
	private int _hauntCount = 0;
	public int HauntCount {
		get { return _hauntCount; }
	}

	private Stopwatch _hauntChargeTimer = new Stopwatch();
	private Stopwatch _hauntProgressTimer = new Stopwatch();
	private Hauntable _activeHauntable = null;

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	public void StartHauntCharge () {
		_hauntChargeTimer.Start();
	}

	public bool CanHaunt {
		get { return _hauntChargeTimer.IsRunning && (_hauntChargeTimer.ElapsedMilliseconds > _hauntChargeDuration * 1000f); }
	}

	public bool IsHaunting {
		get { return _hauntProgressTimer.IsRunning && (_hauntProgressTimer.ElapsedMilliseconds < _hauntDuration * 1000f); }
	}

	public void StartHauntInRoom (Room room) {
		Hauntable hauntable = room.GetComponent<Hauntable>();
		if (CanHaunt && hauntable != null) {
			_activeHauntable = hauntable;
			_activeHauntable.StartHaunting();
			_hauntChargeTimer.Stop();
			_hauntChargeTimer.Reset();
			_hauntProgressTimer.Start();
		}
	}

	void Update () {
		UpdateHauntCount();
	}

	private void UpdateHauntCount () {
		if (_hauntProgressTimer.ElapsedMilliseconds >= _hauntDuration * 1000f) {
			_activeHauntable.StopHaunting();
			_hauntProgressTimer.Stop();
			_hauntProgressTimer.Reset();
			_hauntCount++;
			StartHauntCharge();
		}
	}

}
