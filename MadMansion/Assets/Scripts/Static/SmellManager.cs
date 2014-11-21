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
	private Stopwatch _smellProgressTimer = new Stopwatch();

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
		StartSmellCharge();
	}

	private void StartSmellCharge () {
		_smellChargeTimer.Start();
	}

	public bool CanSmell {
		get { return _smellChargeTimer.IsRunning && (_smellChargeTimer.ElapsedMilliseconds > _smellChargeDuration * 1000f); }
	}

	public bool IsSmelling {
		get { return _smellProgressTimer.IsRunning && (_smellProgressTimer.ElapsedMilliseconds < _maxSmellDuration * 1000f); }
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
		if (_smellProgressTimer.IsRunning) {
			_smellProgressTimer.Reset();
			_smellProgressTimer.Stop();
			_smellCount++;
			Events.g.Raise(new SmellEvent(starting: false));
			StartSmellCharge();
		}
	}

	void Update () {
		UpdateSmellCount();
	}

	private void UpdateSmellCount () {
		if (_smellProgressTimer.IsRunning && _smellProgressTimer.ElapsedMilliseconds >= _maxSmellDuration * 1000f) {
			StopSmelling();
		}
	}

}
