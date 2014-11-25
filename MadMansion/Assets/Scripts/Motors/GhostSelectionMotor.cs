using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[RequireComponent (typeof(CurrRoomFinder))]
public class GhostSelectionMotor : MonoBehaviour {

	[SerializeField]
	private GameObject _ghostSelectionCirclePrefab;
	[SerializeField]
	private float _selectionSensitivity = 0.8f;
	[SerializeField]
	private float _inputDelay = 0.3f;

	private Transform _ghostSelectionCircleTransform = null;
	private Transform _targetTransform;
	private Transform _transform;
	private CurrRoomFinder _currRoomFinder;
	private HashSet<CharacterMotor> _peopleInRoom;

	void Awake () {
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder>();
	}

	public void Initialize () {
	 	GameObject ghostSelectionCircle;
		ghostSelectionCircle = Instantiate(_ghostSelectionCirclePrefab, _transform.position, Quaternion.identity) as GameObject;
		_ghostSelectionCircleTransform = ghostSelectionCircle.transform;
		_peopleInRoom = _currRoomFinder.Room.Characters;
		_targetTransform = _transform;
	}

	private Stopwatch _inputDelayer = new Stopwatch();
	public void AddInput(Vector3 inputVector) {
		Vector3 currPos = _ghostSelectionCircleTransform.position;
		float minimumSqrDistance = Mathf.Infinity;
		Transform chosenTransform = _ghostSelectionCircleTransform;
		foreach (CharacterMotor character in _peopleInRoom) {
			Transform otherTransform = character.transform;
			Vector3 otherPos = otherTransform.position;
			Vector3 toOther = otherPos - currPos;
			float sqrDist = toOther.sqrMagnitude;
			float desire = Vector3.Dot(inputVector, toOther);
			if (desire >= _selectionSensitivity && sqrDist < minimumSqrDistance) {
				chosenTransform = otherTransform;
				minimumSqrDistance = sqrDist;
			}
		}

		print(_inputDelayer.ElapsedMilliseconds);
		if (_inputDelayer.ElapsedMilliseconds >= _inputDelay * 1000f) {
			_inputDelayer.Reset();
			_inputDelayer.Stop();
		}

		if (minimumSqrDistance < Mathf.Infinity && !_inputDelayer.IsRunning) {
			_targetTransform = chosenTransform;
			_inputDelayer.Reset();
			_inputDelayer.Start();
		}
	}

	void LateUpdate () {
		if (_ghostSelectionCircleTransform != null && _targetTransform != null) {
			_ghostSelectionCircleTransform.position = _targetTransform.position;
		}
	}

	public void FinalizeCatch () {
		// _ghostSelectionCircle
	}
}
