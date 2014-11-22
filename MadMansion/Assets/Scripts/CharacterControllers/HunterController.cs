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

	private CharacterMotor _characterMotor;
	private Transform _transform;
	private CurrRoomFinder _currRoomFinder;
	private bool _paused = true;

	void OnEnable ()
	{
		Events.g.AddListener<PauseGameEvent>(PauseInteraction);
		Events.g.AddListener<ResumeGameEvent>(ResumeInteraction);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseInteraction);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeInteraction);
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
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder>();
	}

	void Update () {
		if (_paused) return;
		HandleInput(PlayerInputManager.g.Hunter);
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
			// Events.g.Raise(new PauseGameEvent());
			Debug.Log("Trying to catch");
			TimeManager.g.StartBulletTime();
			CatchManager.g.IsCatching = true; // XXX: Tight coupling!
		} else {
			Debug.Log("Can't Catch");
		}
	}
}