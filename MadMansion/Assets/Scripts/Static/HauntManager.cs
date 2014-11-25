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
	private bool _hauntChargeTimerPaused = false;
	private bool _hauntChargeTimerIsRunning {
		get { return _hauntChargeTimerPaused || _hauntChargeTimer.IsRunning; }
	}
	private Stopwatch _hauntProgressTimer = new Stopwatch();
	private bool _hauntProgressTimerPaused = false;
	private bool _hauntProgressTimerIsRunning {
		get { return _hauntProgressTimerPaused || _hauntProgressTimer.IsRunning; }
	}
	private Hauntable _activeHauntable = null;

	private bool _catchingInProgress = false;

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	void OnEnable ()
	{
		Events.g.AddListener<StartGameEvent>(BeginCharging);
		Events.g.AddListener<PauseGameEvent>(PauseTimers);
		Events.g.AddListener<ResumeGameEvent>(ResumeTimers);
		Events.g.AddListener<CatchEvent>(DisableInteractionOnCatch);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseTimers);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeTimers);
		Events.g.RemoveListener<StartGameEvent>(BeginCharging);
		Events.g.RemoveListener<CatchEvent>(DisableInteractionOnCatch);
	}

	private void DisableInteractionOnCatch (CatchEvent e) {
		if (e.successful) {
			_catchingInProgress = true;

			if (_hauntChargeTimer.IsRunning) {
				_hauntChargeTimer.Stop();
				_hauntChargeTimerPaused = true;
			}

			if (_hauntProgressTimer.IsRunning) {
				_hauntProgressTimer.Stop();
				_hauntProgressTimerPaused = true;
			}
		}
	}

	private void PauseTimers (PauseGameEvent e)
	{
		if (_hauntChargeTimer.IsRunning) {
			_hauntChargeTimer.Stop();
			_hauntChargeTimerPaused = true;
		}

		if (_hauntProgressTimer.IsRunning) {
			_hauntProgressTimer.Stop();
			_hauntProgressTimerPaused = true;
		}
	}

	private void ResumeTimers (ResumeGameEvent e)
	{
		if (_catchingInProgress) { return; }
		if (_hauntChargeTimerPaused) {
			_hauntChargeTimer.Start();
			_hauntChargeTimerPaused = false;
		}

		if (_hauntProgressTimerPaused) {
			_hauntProgressTimer.Start();
			_hauntProgressTimerPaused = false;
		}
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartHauntCharge();
	}

	private void StartHauntCharge () {
		_hauntChargeTimer.Start();
	}

	public bool CanHaunt {
		get { return _hauntChargeTimerIsRunning && (_hauntChargeTimer.ElapsedMilliseconds > _hauntChargeDuration * 1000f); }
	}

	public bool IsHaunting {
		get { return _hauntProgressTimerIsRunning && (_hauntProgressTimer.ElapsedMilliseconds < _hauntDuration * 1000f); }
	}

	public void StartHauntInRoom (Room room) {
		Hauntable hauntable = room.GetComponent<Hauntable>();
		if (CanHaunt && hauntable != null && !_catchingInProgress) {
			_activeHauntable = hauntable;
			_activeHauntable.StartHaunting();
			_hauntChargeTimer.Stop();
			_hauntChargeTimer.Reset();
			_hauntProgressTimer.Start();
			Events.g.Raise(new HauntEvent(starting: true, duration: _hauntDuration));
		}
	}

	void Update () {
		if (_hauntProgressTimerPaused) return;
		UpdateHauntCount();
	}

	private void UpdateHauntCount () {
		if (_hauntProgressTimer.ElapsedMilliseconds >= _hauntDuration * 1000f) {
			_activeHauntable.StopHaunting();
			_hauntProgressTimer.Stop();
			_hauntProgressTimer.Reset();
			_hauntCount++;
			Events.g.Raise(new HauntEvent(starting: false, duration: _hauntDuration));
			StartHauntCharge();
		}
		if (_hauntCount >= _requiredHauntCount) {
			Events.g.Raise(new EndGameEvent(Player.GhostPlayer));
			Events.g.Raise(new PauseGameEvent());
		}
	}

}
