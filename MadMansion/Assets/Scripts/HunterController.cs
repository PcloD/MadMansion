using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(CurrRoomFinder))]
public class HunterController : MonoBehaviour {

	[SerializeField]
	private float _volumeReduction;

	private CharacterMotor _characterMotor;
	private Transform _transform;
	private CurrRoomFinder _currRoomFinder;

	void Awake () {
		_characterMotor = GetComponent<CharacterMotor>();
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder>();
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
		if (smellButton.WasPressed) {
			SmellManager.g.StartSmellInRoomWithCharacterAndVolumeReduction(_currRoomFinder.Room, _characterMotor, _volumeReduction);
		}
		if (smellButton.WasReleased) {
			SmellManager.g.StopSmelling();
		}
		if (catchButton.WasPressed) {
			TryToCatch();
		}
	}

	public void TryToCatch () {
		if (CatchManager.g.CanCatch) {
			Debug.Log("Trying to catch");
			TimeManager.g.StartBulletTime();
			CatchManager.g.IsCatching = true; // XXX: Tight coupling!
		} else {
			Debug.Log("Can't Catch");
		}
	}
}