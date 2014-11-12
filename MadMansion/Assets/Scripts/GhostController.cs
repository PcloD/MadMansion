using UnityEngine;
using System.Collections;
using InControl;

public class GhostController : MonoBehaviour {

	[SerializeField]
	private float _movementSpeed = 10f;
	[SerializeField]
	private float _rotationSpeed = 10f;
	[SerializeField]
	private float _rotationSensitivity = 0.5f;

	private Vector3 _inputVector;
	private Rigidbody _rigidbody;
	private Transform _transform;

	void Awake () {
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;
	}

	void Update () {
		InputDevice device = PlayerInputManager.g.Ghost;

		if (device != null) {
			_inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
		}

		if (_rigidbody.velocity.sqrMagnitude > _rotationSensitivity) {
			transform.forward = Vector3.RotateTowards(transform.forward, _rigidbody.velocity, Time.deltaTime * _rotationSpeed, 0.0001f);
		}
	}

	void FixedUpdate () {
		var relativeVelocity = _inputVector * _movementSpeed;

		// Calcualte the delta velocity
		var currRelativeVelocity = rigidbody.velocity;
		var velocityChange = relativeVelocity - currRelativeVelocity;

		_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	}
}