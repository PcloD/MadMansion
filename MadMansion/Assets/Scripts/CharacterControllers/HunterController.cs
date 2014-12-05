using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(GhostSelectionMotor))]
[RequireComponent (typeof(CurrRoomFinder))]
public class HunterController : MonoBehaviour {

	[SerializeField]
	private float _volumeReduction;
	public float VolumeReduction {
		get { return _volumeReduction; }
	}
	[SerializeField]
	private bool _canAbortSmellPrematurely = false;

	private CharacterMotor _characterMotor;
	private GhostSelectionMotor _ghostSelectionMotor;
	private CurrRoomFinder _currRoomFinder;
	private bool _paused = true;
	private bool _isCatching = false;
	private bool _catchFinalized = false;

	void OnEnable ()
	{
		Events.g.AddListener<PauseGameEvent>(PauseInteraction);
		Events.g.AddListener<ResumeGameEvent>(ResumeInteraction);
		Events.g.AddListener<FinishCatchEvent>(MarkCatchFinalized);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseInteraction);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeInteraction);
		Events.g.RemoveListener<FinishCatchEvent>(MarkCatchFinalized);
	}

	private void MarkCatchFinalized (FinishCatchEvent e) {
		_catchFinalized = true;
	}

	private void PauseInteraction (PauseGameEvent e)
	{
		_paused = true;
	}

	private void ResumeInteraction (ResumeGameEvent e)
	{
		_paused = false;
	}

	void Awake () {
		_characterMotor = GetComponent<CharacterMotor>();
		_currRoomFinder = GetComponent<CurrRoomFinder>();
		_ghostSelectionMotor = GetComponent<GhostSelectionMotor>();
	}

	void Update () {
		if (_paused) return;
		if (_catchFinalized) return;
		if (_isCatching) {
			HandleSelectionInput(PlayerInputManager.g.Hunter);
		} else {
			HandleStandardInput(PlayerInputManager.g.Hunter);
		}
	}

	private void HandleSelectionInput (InputDevice device) {
		if (device == null) {
			return;
		}

		Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
		_ghostSelectionMotor.AddInput(inputVector);

		InputControl catchButton = device.Action4;
		if (catchButton.WasPressed) {
			_ghostSelectionMotor.FinalizeCatch();
		}
	}

	private void HandleStandardInput (InputDevice device) {
		if (device == null) {
			return;
		}
		Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
		_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Hunter);
		InputControl smellButton = device.Action1;
		InputControl catchButton = device.Action4;
		if (smellButton.WasPressed) {
			SmellManager.g.StartSmellInRoomWithHunter(_currRoomFinder.Room, this);
		}
		if (_canAbortSmellPrematurely && smellButton.WasReleased) {
			SmellManager.g.StopSmelling();
		}
		if (catchButton.WasPressed) {
			TryToCatch();
		}
	}

	public void TryToCatch () {
		if (CatchManager.g.CanCatch) {
			Events.g.Raise(new CatchEvent(true));
			_isCatching = true;
			_characterMotor.AddInputWithPriority(Vector3.zero, ControlPriority.Hunter);
			_ghostSelectionMotor.Initialize();
		} else {
			Events.g.Raise(new CatchEvent(false));
		}
	}
}