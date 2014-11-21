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
	private Stopwatch _possessionForcedTimer = new Stopwatch();

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
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<StartGameEvent>(BeginCharging);
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartPossessionCharge();
	}

	private void StartPossessionCharge () {
		_possessionChargeTimer.Start();
	}

	public bool CanPossess {
		get { return _possessionChargeTimer.IsRunning && (_possessionChargeTimer.ElapsedMilliseconds > _possessionChargeDuration * 1000f); }
	}

	public bool MustPossess {
		get { return _possessionForcedTimer.IsRunning && (_possessionForcedTimer.ElapsedMilliseconds > _forcedPossessionSecondsAfterCharge * 1000f); }
	}

	public void StartPossessionInRoomForGhost (Room room, GhostController ghost) {
		if (CanPossess) {

			CharacterMotor character = ghost.GetComponent<CharacterMotor>();
			GhostController closest = room.ClosestGhostToCharacter(character);
			if (closest == null) {
				UnityEngine.Debug.Log("Can't Jump!");
				return;
			}

			UnityEngine.Debug.Log("Jump!");
			closest.enabled = true;
			ghost.enabled = false;

			_possessionCount++;
			_possessionChargeTimer.Stop();
			_possessionChargeTimer.Reset();

			_possessionForcedTimer.Stop();
			_possessionForcedTimer.Reset();

			StartPossessionCharge();
		} else {
			UnityEngine.Debug.Log("Can't Jump!");
		}
	}

	void Update () {
		StartForcedTimerOnceCharged();
	}

	private void StartForcedTimerOnceCharged () {
		if (_possessionChargeTimer.ElapsedMilliseconds >= _possessionChargeDuration * 1000f) {
			_possessionForcedTimer.Start();
		}
	}

}
