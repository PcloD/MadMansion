using UnityEngine;
using System.Collections;
using InControl;

public class ControllerAssigner : MonoBehaviour {
	[SerializeField]
	private GameObject _hunterAssignmentText;
	[SerializeField]
	private GameObject _ghostAssignmentText;

	private InputDevice _hunterDevice;
	private InputDevice _ghostDevice;
	private bool _firstAssignment = true;

	private enum PlayerSelectionStatus {
		AssigningHunter,
		AssigningGhost,
		AllAssigned
	}

	private PlayerSelectionStatus _selectionStatus = PlayerSelectionStatus.AssigningHunter;

	void Awake () {
		InputManager.OnDeviceAttached += inputDevice => ResetControls();
		InputManager.OnDeviceDetached += inputDevice => ResetControls();
	}

	void Start () {
		ResetControls();
	}

	private void ResetControls () {
		_selectionStatus = PlayerSelectionStatus.AssigningHunter;
		_hunterDevice = null;
		_ghostDevice = null;
		_hunterAssignmentText.SetActive(true);
		_ghostAssignmentText.SetActive(false);
		Events.g.Raise(new PauseGameEvent());
	}

	void Update () {
		if (InputManager.ActiveDevice.AnyButton) {
			InputDevice currDevice = InputManager.ActiveDevice;
			if (currDevice == _hunterDevice || currDevice == _ghostDevice) {
				return;
			}
			switch (_selectionStatus) {
				case PlayerSelectionStatus.AllAssigned:
					break;
				case PlayerSelectionStatus.AssigningHunter:
					Debug.Log("Assigning Hunter: " + currDevice.Name);
					Events.g.Raise(new ControllerAssignmentEvent(currDevice, Player.HunterPlayer));
					_hunterDevice = currDevice;
					_selectionStatus = PlayerSelectionStatus.AssigningGhost;
					_hunterAssignmentText.SetActive(false);
					_ghostAssignmentText.SetActive(true);
					break;
				case PlayerSelectionStatus.AssigningGhost:
					Debug.Log("Assigning Ghost: " + currDevice.Name);
					Events.g.Raise(new ControllerAssignmentEvent(currDevice, Player.GhostPlayer));
					_ghostDevice = currDevice;
					_selectionStatus = PlayerSelectionStatus.AllAssigned;
					_ghostAssignmentText.SetActive(false);
					if (_firstAssignment) Events.g.Raise(new StartGameEvent());
					Events.g.Raise(new ResumeGameEvent());
					break;
				default:
					break;
			}
		}

	}
}