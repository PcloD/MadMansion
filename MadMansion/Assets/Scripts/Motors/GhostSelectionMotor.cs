using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[RequireComponent (typeof(CurrRoomFinder))]
[RequireComponent (typeof(CharacterMotor))]
public class GhostSelectionMotor : MonoBehaviour
{

	[SerializeField]
	private GameObject
		_ghostSelectionCirclePrefab;
	[SerializeField]
	private float
		_selectionSensitivity = 0.8f;
	[SerializeField]
	private float
		_inputDelay = 0.3f;

	private Transform _ghostSelectionCircleTransform = null;

	GameObject ghostSelectionCircle;

	private Vector3 _oldPos = Vector3.zero;
	private Transform _targetTransform;
	private Transform _transform;
	private CurrRoomFinder _currRoomFinder;
	private List<Transform> _sortedTransformsInRoom = new List<Transform> ();
	private int _currSelection = 0;
	private CharacterMotor _characterMotor;

	void Awake ()
	{
		_transform = transform;
		_currRoomFinder = GetComponent<CurrRoomFinder> ();
		_characterMotor = GetComponent<CharacterMotor> ();
	}
	void OnEnable ()
	{
		Events.g.AddListener<CatchEndEvent> (StopSelection);
	}
	void OnDisable ()
	{

		Events.g.RemoveListener<CatchEndEvent> (StopSelection);
	}

	public void Initialize ()
	{
		ghostSelectionCircle = Instantiate (_ghostSelectionCirclePrefab, _transform.position, Quaternion.identity) as GameObject;
		_ghostSelectionCircleTransform = ghostSelectionCircle.transform;
		RoomManager.g.DimOtherRooms (exclude: _currRoomFinder.Room);
		HashSet<CharacterMotor> peopleInRoom = _currRoomFinder.Room.Characters;
		_targetTransform = _transform;
		_oldPos = _targetTransform.position;

		HashSet<Transform> exclude = new HashSet<Transform> ();
		Transform currTransform = _transform;
		_sortedTransformsInRoom.Add (currTransform);
		exclude.Add (currTransform);
		while (exclude.Count < peopleInRoom.Count) {
			currTransform = FindClosestTransformNotInSet (currTransform.position, peopleInRoom, exclude);
			_sortedTransformsInRoom.Add (currTransform);
			exclude.Add (currTransform);
		}
	}

	public Transform FindClosestTransformNotInSet (Vector3 target, HashSet<CharacterMotor> allCharacters, HashSet<Transform> exclude)
	{
		float minimumSqrDistance = Mathf.Infinity;
		Transform closest = null;
		foreach (CharacterMotor character in allCharacters) {
			Transform otherTransform = character.transform;
			Vector3 otherPos = otherTransform.position;
			Vector3 toOther = otherPos - target;
			float sqrDist = toOther.sqrMagnitude;
			if (sqrDist < minimumSqrDistance && !exclude.Contains (otherTransform)) {
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

	private Stopwatch _inputDelayer = new Stopwatch ();
	public void AddInput (Vector3 inputVector)
	{
		if (_inputDelayer.ElapsedMilliseconds >= _inputDelay * 1000f) {
			_inputDelayer.Reset ();
			_inputDelayer.Stop ();
		}
		if (_inputDelayer.IsRunning || inputVector.sqrMagnitude < _selectionSensitivity) {
			return;
		}
		int delta = -1;
		Vector3 upRight = (new Vector3 (1f, 0f, 1f)).normalized;
		Vector3 bottomLeft = (new Vector3 (-1f, 0f, -1f)).normalized;
		if (Vector3.Dot (inputVector, upRight) > Vector3.Dot (inputVector, bottomLeft)) {
			delta = 1;
			_inputDelayer.Reset ();
			_inputDelayer.Start ();
		}
		_currSelection = Modulo (_currSelection + delta, _sortedTransformsInRoom.Count);
		if (_targetTransform != null) {
			_targetTransform.position = _oldPos;
		}
		_targetTransform = _sortedTransformsInRoom [_currSelection];
		_oldPos = _targetTransform.position;
		_inputDelayer.Reset ();
		_inputDelayer.Start ();
	}

	void LateUpdate ()
	{
		if (_ghostSelectionCircleTransform != null && _targetTransform != null) {
			_ghostSelectionCircleTransform.position = _targetTransform.position;
			if (_inputDelayer.IsRunning) {
				_targetTransform.position = _oldPos + Vector3.up * 3f * _inputDelayer.ElapsedMilliseconds / 1000f;
			}
		}
	}

	void StopSelection (CatchEndEvent e)
	{
		if (e.catchRight) {
		} else {
			Destroy (ghostSelectionCircle);
			RoomManager.g.UndimRoom (room: _currRoomFinder.Room);
		}

	}

	public void FinalizeCatch ()//finalize means the hunter player finishes choosing. End it goes to CatchEnd
	{
		Events.g.Raise (new FinishCatchEvent (hunter: _characterMotor, guess: _targetTransform.GetComponent<CharacterMotor> ()));
	}
}
