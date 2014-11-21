using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class SmellManager : MonoBehaviour {

	public static SmellManager g;
	[SerializeField]
	private float _smellChargeDuration = 4f;
	[SerializeField]
	private float _maxSmellDuration = 4f;
	public float SmellTimerPercentage {
		get {
			return Mathf.Min(1f, _smellProgressTimer.ElapsedMilliseconds/(_maxSmellDuration * 1000f));
		}
	}
	public float SmellChargePercentage {
		get {
			return Mathf.Min(1f, _smellChargeTimer.ElapsedMilliseconds/(_smellChargeDuration * 1000f));
		}
	}

	private int _smellCount = 0;
	public int SmellCount {
		get { return _smellCount; }
	}

	private Stopwatch _smellChargeTimer = new Stopwatch();
	private bool _smellChargeTimerPaused = false;
	private bool _smellChargeTimerIsRunning {
		get { return _smellChargeTimerPaused || _smellChargeTimer.IsRunning; }
	}
	private Stopwatch _smellProgressTimer = new Stopwatch();
	private bool _smellProgressTimerPaused = false;
	private bool _smellProgressTimerIsRunning {
		get { return _smellProgressTimerPaused || _smellProgressTimer.IsRunning; }
	}

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
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseTimers);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeTimers);
		Events.g.RemoveListener<StartGameEvent>(BeginCharging);
	}

	private void PauseTimers (PauseGameEvent e)
	{
		if (_smellChargeTimer.IsRunning) {
			_smellChargeTimer.Stop();
			_smellChargeTimerPaused = true;
		}

		if (_smellProgressTimer.IsRunning) {
			_smellProgressTimer.Stop();
			_smellProgressTimerPaused = true;
		}
	}

	private void ResumeTimers (ResumeGameEvent e)
	{
		if (_smellChargeTimerPaused) {
			_smellChargeTimer.Start();
			_smellChargeTimerPaused = false;
		}

		if (_smellProgressTimerPaused) {
			_smellProgressTimer.Start();
			_smellProgressTimerPaused = false;
		}
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartSmellCharge();
	}

	private void StartSmellCharge () {
		_smellChargeTimer.Start();
	}

	public bool CanSmell {
		get { return _smellChargeTimerIsRunning && (_smellChargeTimer.ElapsedMilliseconds > _smellChargeDuration * 1000f); }
	}

	public bool IsSmelling {
		get { return _smellProgressTimerIsRunning && (_smellProgressTimer.ElapsedMilliseconds < _maxSmellDuration * 1000f); }
	}

	public void StartSmellInRoomWithHunter (Room room, HunterController character) {
		if (CanSmell && GhostTracker.g.CanSeeHistory && !IsSmelling) {
			_smellChargeTimer.Stop();
			_smellChargeTimer.Reset();
			_smellProgressTimer.Start();

			Events.g.Raise(new SmellEvent(starting: true, room: room, hunter: character));
		}
	}

	public void StopSmelling () {
		if (_smellProgressTimerIsRunning) {
			_smellProgressTimer.Reset();
			_smellProgressTimer.Stop();
			_smellCount++;
			Events.g.Raise(new SmellEvent(starting: false));
			StartSmellCharge();
		}
	}

	void Update () {
		if (_smellProgressTimerPaused) return;
		UpdateSmellCount();
	}

	private void UpdateSmellCount () {
		if (_smellProgressTimerIsRunning && _smellProgressTimer.ElapsedMilliseconds >= _maxSmellDuration * 1000f) {
			StopSmelling();
		}
	}

}
