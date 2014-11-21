using UnityEngine;
using System.Collections;
using InControl;

public class HunterController : MonoBehaviour {

	[SerializeField]
	private float volumeReduction;

	private CharacterMotor _characterMotor;
	private Transform _transform;

	void Awake () {
		_characterMotor = GetComponent<CharacterMotor>();
		_transform = transform;
	}

	void Update () {
		InputDevice device = PlayerInputManager.g.Hunter;
		HandleInput(device);
	}

	private void HandleInput (InputDevice device) {
		if (device == null) {
			return;
		}
			Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
			_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Hunter);

		InputControl smellButton = device.Action1;
		InputControl catchButton = device.Action4;
		if (smellButton.IsPressed) {
			Smell();
		} else {
			StopSmelling();
		}
		if (catchButton.WasPressed) {
			TryToCatch();
		}
		if (catchButton.WasReleased) {
			StopTryingToCatch();
		}
	}

	private void Smell () {
		if (GhostTracker.g.CanSeeHistory) {
			Vector3 toOldGhostPos = (_transform.position - GhostTracker.g.HistoricalLocation);
			float volumeScale = Mathf.Min(volumeReduction/toOldGhostPos.magnitude, 1f);
			SoundManager.g.PlayGhostSound(volumeScale);
		}
	}

	private void StopSmelling () {
		SoundManager.g.StopGhostSound();
	}

	private void TryToCatch () {
		TimeManager.g.StartBulletTime();
	}

	private void StopTryingToCatch () {
		TimeManager.g.StopBulletTime();
	}
}