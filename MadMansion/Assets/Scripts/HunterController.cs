using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(CurrRoomFinder))]
public class HunterController : MonoBehaviour {

	[SerializeField]
	private float _volumeReduction;
	public float VolumeReduction {
		get { return _volumeReduction; }
	}

	private InputDevice _device;
	private CharacterMotor _characterMotor;
	private Transform _transform;
	private CurrRoomFinder _currRoomFinder;
	private bool _paused = true;

	void OnEnable ()
	{
		Events.g.AddListener<PauseGameEvent>(PauseInteraction);
		Events.g.AddListener<ResumeGameEvent>(ResumeInteraction);
		Events.g.AddListener<ControllerAssignmentEvent>(AssignController);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseInteraction);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeInteraction);
		Events.g.RemoveListener<ControllerAssignmentEvent>(AssignController);
	}

	private void PauseInteraction (PauseGameEvent e)
	{
		_paused = true;
	}

	private void ResumeInteraction (ResumeGameEvent e)
	{
		_paused = false;
	}

	private void AssignController (ControllerAssignmentEvent e)
	{
		if (e.player == Player.HunterPlayer) {
			_device = e.device;
		}
	}

	void Awake () {
		_characterMotor = GetComponent<CharacterMotor>();
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder>();
	}

	void Update () {
		if (_paused) return;
		HandleInput();
	}

	private void HandleInput () {
		if (_device == null) {
			return;
		}
			Vector3 inputVector = new Vector3(_device.LeftStickX.Value, 0f, _device.LeftStickY.Value);
			_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Hunter);

		InputControl smellButton = _device.Action1;
		InputControl catchButton = _device.Action4;
		if (smellButton.WasPressed) {
			SmellManager.g.StartSmellInRoomWithHunter(_currRoomFinder.Room, this);
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