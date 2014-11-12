using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInputManager : MonoBehaviour {
	[SerializeField]
	private GameObject _hunterAssignmentText;
	[SerializeField]
	private GameObject _ghostAssignmentText;

	public static PlayerInputManager g;

	private InputDevice _hunter = null;
	private InputDevice _ghost = null;

	public InputDevice Hunter {
		get { return _hunter; }
	}

	public InputDevice Ghost {
		get { return _ghost; }
	}

	private enum PlayerSelectionStatus {
		AssigningHunter,
		AssigningGhost,
		AllAssigned
	}

	private PlayerSelectionStatus _selectionStatus = PlayerSelectionStatus.AssigningHunter;

	void Awake () {
		if (g == null) {
			g = this;

			InputManager.OnActiveDeviceChanged += inputDevice => SetupController(inputDevice);
		} else {
			Destroy(this);
		}
	}

	void Start () {
		_hunterAssignmentText.SetActive(true);
		_ghostAssignmentText.SetActive(false);
	}

	private void SetupController(InputDevice controller) {
		switch (_selectionStatus) {
			case PlayerSelectionStatus.AssigningHunter:
				Debug.Log("Assigning Hunter: " + controller.Name);
				_hunter = controller;
				_selectionStatus = PlayerSelectionStatus.AssigningGhost;
				_hunterAssignmentText.SetActive(false);
				_ghostAssignmentText.SetActive(true);
				break;
			case PlayerSelectionStatus.AssigningGhost:
				Debug.Log("Assigning Ghost: " + controller.Name);
				_ghost = controller;
				_selectionStatus = PlayerSelectionStatus.AllAssigned;
				_ghostAssignmentText.SetActive(false);
				break;
			default:
				break;
		}
	}
}