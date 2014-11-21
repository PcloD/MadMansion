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

	public void StartSmellCharge () {
		_smellChargeTimer.Start();
	}

	public bool CanSmell {
		get { return _smellChargeTimer.IsRunning && (_smellChargeTimer.ElapsedMilliseconds > _smellChargeDuration * 1000f); }
	}

	public bool IsSmelling {
		get { return _smellProgressTimer.IsRunning && (_smellProgressTimer.ElapsedMilliseconds < _maxSmellDuration * 1000f); }
	}

	private float _volumeReduction = 1f;
	private CharacterMotor _character = null;
	public void StartSmellInRoomWithCharacterAndVolumeReduction (Room room, CharacterMotor character, float volumeReduction) {
		if (CanSmell && GhostTracker.g.CanSeeHistory && !IsSmelling) {
			_character = character;
			_volumeReduction = volumeReduction;

			_smellChargeTimer.Stop();
			_smellChargeTimer.Reset();
			_smellProgressTimer.Start();
		}
	}

	public void StopSmelling () {
		if (_smellProgressTimer.IsRunning) {
			SoundManager.g.StopGhostSound();
			_smellProgressTimer.Reset();
			_smellProgressTimer.Stop();
			_smellCount++;
			StartSmellCharge();
		}
	}

	void Update () {
		UpdateHauntCount();
	}

	private void UpdateHauntCount () {
		if (_smellProgressTimer.IsRunning && _smellProgressTimer.ElapsedMilliseconds >= _maxSmellDuration * 1000f) {
			StopSmelling();
		} else if (_smellProgressTimer.IsRunning) {
			Vector3 toOldGhostPos = (_character.transform.position - GhostTracker.g.HistoricalLocation);
			float volumeScale = Mathf.Min(_volumeReduction/toOldGhostPos.magnitude, 1f);
			SoundManager.g.PlayGhostSound(volumeScale);
		}
	}

}
