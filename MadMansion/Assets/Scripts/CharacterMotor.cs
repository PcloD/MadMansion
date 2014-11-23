using UnityEngine;
using System.Collections;
using InControl;

public enum ControlPriority {
	Ghost,
	Hunter,
	NPC
}

public class CharacterMotor : MonoBehaviour {

	[SerializeField]
	private float _movementSpeed = 7f;
	public float MovementSpeed {
		get { return _movementSpeed; }
	}
	[SerializeField]
	private float _rotationSpeed = 10f;
	[SerializeField]
	private float _rotationSensitivity = 0.5f;

	[SerializeField]
	private float _ghostMovementSensitivity = 0.5f;

	private GhostController _ghostController;
	private HunterController _hunterController;

	private Vector3 _ghostInputVector;
	private Vector3 _hunterInputVector;
	private Vector3 _standardInputVector;
	private Rigidbody _rigidbody;
	private Transform _transform;
	private bool _paused = true;

	public bool IsPossessed {
		get { return _ghostController.enabled; }
	}

	public bool IsHunter {
		get { return _hunterController.enabled; }
	}

	public void AddInputWithPriority (Vector3 input, ControlPriority priority) {
		input.y = 0f;
		switch (priority) {
			case ControlPriority.Ghost:
				_ghostInputVector = input;
				break;
			case ControlPriority.Hunter:
				_hunterInputVector = input;
				break;
			default:
				_standardInputVector = input;
				break;
		}
	}

	void Awake () {
		_rigidbody = GetComponent<Rigidbody>();
		_ghostController = GetComponent<GhostController>();
		_hunterController = GetComponent<HunterController>();
		_transform = transform;
	}

	void OnEnable ()
	{
		Events.g.AddListener<PauseGameEvent>(PauseMovement);
		Events.g.AddListener<ResumeGameEvent>(ResumeMovement);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent>(PauseMovement);
		Events.g.RemoveListener<ResumeGameEvent>(ResumeMovement);
	}

	private void PauseMovement (PauseGameEvent e)
	{
		_paused = true;
	}

	private void ResumeMovement (ResumeGameEvent e)
	{
		_paused = false;
	}

	void Update () {
		if (_paused) return;
		// Rotate towards velocity vector
		if (_rigidbody.velocity.sqrMagnitude > _rotationSensitivity) {
			_transform.forward = Vector3.RotateTowards(_transform.forward, _rigidbody.velocity, Time.deltaTime * (_rotationSpeed * TimeScale), 0.0001f);
		}
	}

	void FixedUpdate () {
		if (_paused) return;
		Vector3 inputVector = _standardInputVector;
		if (IsHunter && (!IsPossessed || _ghostInputVector.sqrMagnitude < _ghostMovementSensitivity)) {
			inputVector = _hunterInputVector;
		} else if (IsPossessed) {
			inputVector = _ghostInputVector;
		}
		var relativeVelocity = inputVector.normalized * (_movementSpeed * TimeScale);

		// Calcualte the delta velocity
		var currRelativeVelocity = rigidbody.velocity;
		var velocityChange = relativeVelocity - currRelativeVelocity;

		_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	}

	private float TimeScale {
		get {
			if (IsHunter) {
				return TimeManager.g.HunterTimeScale;
			} else {
				return TimeManager.g.TimeScale;
			}
		}
	}
}