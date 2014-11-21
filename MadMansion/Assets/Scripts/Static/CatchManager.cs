using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CatchManager : MonoBehaviour {

	public static CatchManager g;
	[SerializeField]
	private float _catchChargeDuration = 4f;
	public float CatchChargePercentage {
		get {
			return Mathf.Min(1f, _catchChargeTimer.ElapsedMilliseconds/(_catchChargeDuration * 1000f));
		}
	}

	private Stopwatch _catchChargeTimer = new Stopwatch();
	private bool _catchChargeTimerPaused = false;
	private bool _catchChargeTimerIsRunning {
		get { return _catchChargeTimerPaused || _catchChargeTimer.IsRunning; }
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
		if (_catchChargeTimer.IsRunning) {
			_catchChargeTimer.Stop();
			_catchChargeTimerPaused = true;
		}
	}

	private void ResumeTimers (ResumeGameEvent e)
	{
		if (_catchChargeTimerPaused) {
			_catchChargeTimer.Start();
			_catchChargeTimerPaused = false;
		}
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartCatchCharge();
	}

	private bool _isCatching = false;
	public bool IsCatching {
		get { return _isCatching; }
		set { _isCatching = value; } // XXX: Tight coupling
	}

	private void StartCatchCharge () {
		_catchChargeTimer.Start();
	}

	public bool CanCatch {
		get { return _catchChargeTimerIsRunning && (_catchChargeTimer.ElapsedMilliseconds > _catchChargeDuration * 1000f); }
	}
}