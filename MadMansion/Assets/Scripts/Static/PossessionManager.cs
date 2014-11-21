using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class PossessionManager : MonoBehaviour {

	public static PossessionManager g;
	[SerializeField]
	private float _possessionChargeDuration = 4f;
	[SerializeField]
	private float _forcedPossessionSecondsAfterCharge = 4f;
	[SerializeField]
	private float _fullyRevealedAfterSeconds = 4f;
	public float PossessionForcedPercentage {
		get {
			return Mathf.Min(1f, _possessionForcedTimer.ElapsedMilliseconds/(_forcedPossessionSecondsAfterCharge * 1000f));
		}
	}
	public float PossessionChargePercentage {
		get {
			return Mathf.Min(1f, _possessionChargeTimer.ElapsedMilliseconds/(_possessionChargeDuration * 1000f));
		}
	}
	public float RevealedPercentage {
		get {
			if (!MustPossess) {
				return 0f;
			}
			return Mathf.Min(1f,(_possessionForcedTimer.ElapsedMilliseconds - _forcedPossessionSecondsAfterCharge * 1000f)/(_fullyRevealedAfterSeconds * 1000f));
		}
	}
	private int _possessionCount = 0;
	public int PossessionCount {
		get { return _possessionCount; }
	}

	private Stopwatch _possessionChargeTimer = new Stopwatch();
	private bool _possessionChargeTimerPaused = false;
	private bool _possessionChargeTimerIsRunning {
		get { return _possessionChargeTimerPaused || _possessionChargeTimer.IsRunning; }
	}
	private Stopwatch _possessionForcedTimer = new Stopwatch();
	private bool _possessionForcedTimerPaused = false;
	private bool _possessionForcedTimerIsRunning {
		get { return _possessionForcedTimerPaused || _possessionForcedTimer.IsRunning; }
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
		if (_possessionChargeTimer.IsRunning) {
			_possessionChargeTimer.Stop();
			_possessionChargeTimerPaused = true;
		}

		if (_possessionForcedTimer.IsRunning) {
			_possessionForcedTimer.Stop();
			_possessionForcedTimerPaused = true;
		}
	}

	private void ResumeTimers (ResumeGameEvent e)
	{
		if (_possessionChargeTimerPaused) {
			_possessionChargeTimer.Start();
			_possessionChargeTimerPaused = false;
		}

		if (_possessionForcedTimerPaused) {
			_possessionForcedTimer.Start();
			_possessionForcedTimerPaused = false;
		}
	}

	private void BeginCharging (StartGameEvent e)
	{
		StartPossessionCharge();
	}

	private void StartPossessionCharge () {
		_possessionChargeTimer.Start();
	}

	public bool CanPossess {
		get { return _possessionChargeTimerIsRunning && (_possessionChargeTimer.ElapsedMilliseconds > _possessionChargeDuration * 1000f); }
	}

	public bool MustPossess {
		get { return _possessionForcedTimerIsRunning && (_possessionForcedTimer.ElapsedMilliseconds > _forcedPossessionSecondsAfterCharge * 1000f); }
	}

	public void StartPossessionInRoomForGhost (Room room, GhostController ghost) {
		if (CanPossess) {

			CharacterMotor character = ghost.GetComponent<CharacterMotor>();
			GhostController closest = room.ClosestGhostToCharacter(character);
			if (closest == null) {
				Events.g.Raise(new PossessionEvent(false));
				return;
			}

			Events.g.Raise(new PossessionEvent(true));
			closest.enabled = true;
			ghost.enabled = false;

			_possessionCount++;
			_possessionChargeTimer.Stop();
			_possessionChargeTimer.Reset();

			_possessionForcedTimer.Stop();
			_possessionForcedTimer.Reset();

			StartPossessionCharge();
		} else {
			Events.g.Raise(new PossessionEvent(false));
		}
	}

	void Update () {
		if (_possessionChargeTimerPaused) return;
		StartForcedTimerOnceCharged();
	}

	private void StartForcedTimerOnceCharged () {
		if (_possessionChargeTimer.ElapsedMilliseconds >= _possessionChargeDuration * 1000f) {
			_possessionForcedTimer.Start();
		}
	}

}
