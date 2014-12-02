using UnityEngine;
using System.Collections;

public class FireParticle : MonoBehaviour {

	private float _lifetime = 2f;
	private float _age = 0f;
	private float _speed = 0f;
	private float _acceleration = 0f;
	private Transform _transform;
	private AnimationCurve _sizeCurve;


	void Awake () {
		_transform = transform;
	}

	public void Init (float lifetime, float initialSpeed, float acceleration, AnimationCurve sizeCurve) {
		_age = 0f;
		_lifetime = lifetime;
		_speed = initialSpeed;
		_acceleration = acceleration;
		_sizeCurve = sizeCurve;
	}

	public bool NeedsRecycling {
		get { return _age >= _lifetime; }
	}

	void Update () {
		_age += Time.deltaTime;
		_speed += Time.deltaTime * _acceleration;
		_transform.position += Time.deltaTime * Vector3.up * _speed;
		_transform.localScale = Vector3.one * _sizeCurve.Evaluate(_age/_lifetime);
	}
}
