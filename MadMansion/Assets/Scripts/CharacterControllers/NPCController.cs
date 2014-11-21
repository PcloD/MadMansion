using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CurrRoomFinder))]
public class NPCController : MonoBehaviour {

	[SerializeField]
	private float _destinationSensitivity = 0.5f;

	private Vector3 _currDest;
	private Transform _transform;
	private CharacterMotor _characterMotor;
	private List<Room> _roomPattern;
	private int _currRoomIndex = 0;
	private CurrRoomFinder _currRoomFinder;
	private List<Vector3> _destList;

	void Awake () {
		Cache();
	}

	void Start () {
		StartCoroutine (MovementLoop());
	}

	public void InitRoomPattern (List<Room> pattern) {
		_roomPattern = pattern;
	}

	private IEnumerator MovementLoop () {
		yield return new WaitForSeconds(Random.Range(0f,1f));
		while (true) {
			PickDest();
			yield return new WaitForSeconds(Random.Range(0f,5f));
			yield return StartCoroutine(ContinuouslySteerToDest ());
		}
	}

	private void PickDest () {
		if (_destList == null || _destList.Count == 0) {
			Room nextRoom = _roomPattern[_currRoomIndex];
			_destList = RoomManager.g.PathBetweenRooms(_currRoomFinder.Room, nextRoom);
			_currRoomIndex++;
			_currRoomIndex %= _roomPattern.Count;
		}

		_currDest = _destList[0];
		_destList.RemoveAt(0);
	}

	private IEnumerator ContinuouslySteerToDest () {
		YieldInstruction wait = new WaitForFixedUpdate();
		float jitterTimer = 0f;
		float jitterTimerDuration = 0.5f;
		Vector3 inputVector = Vector3.zero;
		Vector3 offsetVector = Vector3.zero;
		Vector3 initialVector = _currDest - _transform.position;
		do {
			Debug.DrawLine(_transform.position, _currDest);
			inputVector = (_currDest - _transform.position);
			//float completion = inputVector.sqrMagnitude/initialVector.sqrMagnitude;
			inputVector = Vector3.Lerp(inputVector, inputVector + offsetVector, jitterTimer);
			jitterTimer += Time.fixedDeltaTime;
			if (jitterTimer > jitterTimerDuration) {
				Vector3 perp = Vector3.Cross(initialVector, Vector3.up).normalized * 2f;
				Debug.DrawLine(_transform.position, _transform.position + perp);
				offsetVector = perp * (float)Random.Range(-1, 2);
				jitterTimerDuration = Random.Range(0.5f,2f);
				jitterTimer = 0f;
			}
			// inputVector = Vector3.ClampMagnitude(inputVector, completion);
			_characterMotor.AddInputWithPriority(inputVector, ControlPriority.NPC);
			yield return wait;
		} while ((_currDest - _transform.position).sqrMagnitude > _destinationSensitivity);
		_characterMotor.AddInputWithPriority(Vector3.zero, ControlPriority.NPC);
		yield break;
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
		if (!this.enabled) {
			return;
		}
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(_currDest, 0.2f);
	}
}
