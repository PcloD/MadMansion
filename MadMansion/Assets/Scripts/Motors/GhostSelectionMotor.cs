using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[RequireComponent (typeof(CurrRoomFinder))]
[RequireComponent (typeof(CharacterMotor))]
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
	private List<Transform> _sortedTransformsInRoom = new List<Transform>();
	private int _currSelection = 0;
	private CharacterMotor _characterMotor;

	void Awake () {
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder>();
		_characterMotor = GetComponent<CharacterMotor>();
	}

	public void Initialize () {
	 	GameObject ghostSelectionCircle;
		ghostSelectionCircle = Instantiate(_ghostSelectionCirclePrefab, _transform.position, Quaternion.identity) as GameObject;
		_ghostSelectionCircleTransform = ghostSelectionCircle.transform;
		HashSet<CharacterMotor> peopleInRoom = _currRoomFinder.Room.Characters;
		_targetTransform = _transform;

		HashSet<Transform> exclude = new HashSet<Transform>();
		Transform currTransform = _transform;
		_sortedTransformsInRoom.Add (currTransform);
		exclude.Add(currTransform);
		while (exclude.Count < peopleInRoom.Count) {
			currTransform = FindClosestTransformNotInSet(currTransform.position, peopleInRoom, exclude);
			_sortedTransformsInRoom.Add (currTransform);
			exclude.Add(currTransform);
		}
	}

	public Transform FindClosestTransformNotInSet(Vector3 target, HashSet<CharacterMotor> allCharacters, HashSet<Transform> exclude) {
		float minimumSqrDistance = Mathf.Infinity;
		Transform closest = null;
		foreach (CharacterMotor character in allCharacters) {
			Transform otherTransform = character.transform;
			Vector3 otherPos = otherTransform.position;
			Vector3 toOther = otherPos - target;
			float sqrDist = toOther.sqrMagnitude;
			if (sqrDist < minimumSqrDistance && !exclude.Contains(otherTransform)) {
				closest = otherTransform;
				minimumSqrDistance = sqrDist;
			}
		}

		return closest;
	}

	private int Modulo (int x, int m)
	{
		return x < 0 ? ((x % m) + m) % m : x % m;
	}

	private Stopwatch _inputDelayer = new Stopwatch();
	public void AddInput(Vector3 inputVector) {
		if (_inputDelayer.ElapsedMilliseconds >= _inputDelay * 1000f) {
			_inputDelayer.Reset();
			_inputDelayer.Stop();
		}
		if (_inputDelayer.IsRunning || inputVector.sqrMagnitude < _selectionSensitivity) {
			return;
		}
		int delta = -1;
		Vector3 upRight = (new Vector3(1f,0f,1f)).normalized;
		Vector3 bottomLeft = (new Vector3(-1f,0f,-1f)).normalized;
		if (Vector3.Dot(inputVector, upRight) > Vector3.Dot(inputVector, bottomLeft)) {
			delta = 1;
			_inputDelayer.Reset();
			_inputDelayer.Start();
		}
		_currSelection = Modulo(_currSelection + delta, _sortedTransformsInRoom.Count);
		_targetTransform = _sortedTransformsInRoom[_currSelection];
		_inputDelayer.Reset();
		_inputDelayer.Start();
	}

	void LateUpdate () {
		if (_ghostSelectionCircleTransform != null && _targetTransform != null) {
			_ghostSelectionCircleTransform.position = _targetTransform.position;
		}
	}

	public void FinalizeCatch () {
		Events.g.Raise(new FinishCatchEvent(hunter: _characterMotor, guess: _targetTransform.GetComponent<CharacterMotor>()));
	}
}
