using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(CurrRoomFinder))]
public class GhostController : MonoBehaviour {
	private Transform _transform;
	private CharacterMotor _characterMotor;
	private bool _paused = true;

	[SerializeField]
	private Renderer _revealedGhostRenderer;
	[SerializeField]
	private Light _revealedGhostLight;
	[SerializeField]
	private AnimationCurve _lightRevealCurve;
	[SerializeField]
	private AnimationCurve _transparencyRevealCurve;

	private CurrRoomFinder _currRoomFinder;

	void Awake () {
		Cache();
	}

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

	void Update () {
		if (_paused) return;
		HandleInput(PlayerInputManager.g.Ghost);
		RevealGhost();
	}

	private void RevealGhost () {
		float revealedPercentage = PossessionManager.g.RevealedPercentage;
		Color color = _revealedGhostRenderer.material.color;
		color.a = _transparencyRevealCurve.Evaluate(revealedPercentage);
		_revealedGhostRenderer.material.color = color;
		_revealedGhostLight.intensity = _lightRevealCurve.Evaluate(revealedPercentage);
	}

	void FixedUpdate () {
		if (_paused) return;
		GhostTracker.g.RecordLocation(_transform.position);
	}

	private void HandleInput (InputDevice device) {
		if (device == null) {
			return;
		}
		Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
		_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Ghost);

		InputControl possessionButton = device.Action1;
		InputControl hauntButton = device.Action3;
		if (possessionButton.WasPressed) {
			JumpToClosest();
		}
		if (hauntButton.WasPressed) {
			HauntManager.g.StartHauntInRoom(_currRoomFinder.Room);
		}
	}

	private void JumpToClosest () {
		PossessionManager.g.StartPossessionInRoomForGhost(_currRoomFinder.Room, this);
	}

	private void Cache () {
		_transform = transform;
		_characterMotor = GetComponent<CharacterMotor>();
		_currRoomFinder = GetComponent<CurrRoomFinder>();
	}

	private bool NeedToCache {
		get { return (_transform == null || _characterMotor == null || _currRoomFinder == null); }
	}

	void OnDrawGizmos () {
		if (NeedToCache) {
			Cache();
		}
		if (!this.enabled) {
			return;
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere (_transform.position, 0.1f);
	}
}