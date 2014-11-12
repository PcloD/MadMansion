using UnityEngine;
using System.Collections;
using InControl;

public class HunterController : MonoBehaviour {

	private CharacterMotor _characterMotor;

	void Awake () {
		_characterMotor = GetComponent<CharacterMotor>();
	}

	void Update () {
		InputDevice device = PlayerInputManager.g.Hunter;
		if (device != null) {
			Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
			_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Hunter);
		}
	}
}