using UnityEngine;
using System.Collections;

public class PulseComponent : MonoBehaviour {

	private Transform _transform;
	[SerializeField]
	private float _speed = 10f;
	[SerializeField]
	private float _scale = 0.5f;
	[SerializeField]
	private float _initialScale = 0.5f;


	void Awake () {
		_transform = transform;
	}

	float counter = 0f;
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		_transform.localScale = Vector3.one * (_initialScale+Mathf.PingPong(counter * _speed, 1f)* _scale);
	}
}
