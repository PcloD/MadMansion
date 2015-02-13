using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInputManager : MonoBehaviour {
	public static PlayerInputManager g;

	[SerializeField]
	private GameObject _hunterAssignmentText;
	[SerializeField]
	private GameObject _ghostAssignmentText;

	private InputDevice _hunterDevice;
	public InputDevice Hunter {
		get { return _hunterDevice; }
	}
	private InputDevice _ghostDevice;
	public InputDevice Ghost {
		get { return _ghostDevice; }
	}
	private bool _firstAssignment = true;

	private enum PlayerSelectionStatus {
		AssigningHunter,
		AssigningGhost,
		AllAssigned
	}

	private PlayerSelectionStatus _selectionStatus = PlayerSelectionStatus.AssigningHunter;

	void OnEnable ()
	{
		Events.g.AddListener<StartCharacterSelectionEvent>(BeginSelection);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<StartCharacterSelectionEvent>(BeginSelection);
	}

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	void BeginSelection (StartCharacterSelectionEvent e) {
		InputManager.OnDeviceAttached += inputDevice => ResetControls();
		InputManager.OnDeviceDetached += inputDevice => ResetControls();
		StartCoroutine(ManageAssignment(e.gameMode));
	}

	private void ResetControls () {
		_selectionStatus = PlayerSelectionStatus.AssigningHunter;
		_hunterDevice = null;
		_ghostDevice = null;
		_hunterAssignmentText.SetActive(true);
		_ghostAssignmentText.SetActive(false);
		Events.g.Raise(new PauseGameEvent());
	}


	IEnumerator ManageAssignment (GameMode gameMode) {
		yield return new WaitForSeconds(0.5f);
		ResetControls();
		while (true) {
			if (InputManager.ActiveDevice.AnyButton) {
				InputDevice currDevice = InputManager.ActiveDevice;
				if (currDevice == _hunterDevice || currDevice == _ghostDevice) {
					yield return null;
					continue;
				}
				switch (_selectionStatus) {
					case PlayerSelectionStatus.AllAssigned:
						break;
					case PlayerSelectionStatus.AssigningHunter:
						Debug.Log("Assigning Hunter: " + currDevice.Name); // TODO: Strip out debugs!
						_hunterDevice = currDevice;
						_selectionStatus = PlayerSelectionStatus.AssigningGhost;
						_hunterAssignmentText.SetActive(false);
						_ghostAssignmentText.SetActive(true);
						break;
					case PlayerSelectionStatus.AssigningGhost:
						Debug.Log("Assigning Ghost: " + currDevice.Name);
						_ghostDevice = currDevice;
						_selectionStatus = PlayerSelectionStatus.AllAssigned;
						_ghostAssignmentText.SetActive(false);
						Events.g.Raise(new ResumeGameEvent());
						if (_firstAssignment) {
							Events.g.Raise(new StartGameEvent(gameMode));
							_firstAssignment = false;
						}
						break;
					default:
						break;
				}
			}
			yield return null;
		}
	}
}