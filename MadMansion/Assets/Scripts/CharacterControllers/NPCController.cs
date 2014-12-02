using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadMansion.AI;
using MadMansion.Utils.Math;

[RequireComponent (typeof(CurrRoomFinder))]
public class NPCController : MonoBehaviour {

	[SerializeField]
	private float _destinationSensitivity = 0.5f;

	[SerializeField]
	private AnimationCurve _pauseCurve;
	[SerializeField]
	private int _contextMapLOD = 8;
	[SerializeField]
	private float _collisionAvoidanceRadius = 2f;

	private Vector3 _currDest;
	private Transform _transform;
	private CharacterMotor _characterMotor;
	private List<IFurniture> _furniturePattern;
	private int _currFurnitureIndex = 0;
	private CurrRoomFinder _currRoomFinder;
	private List<Vector3> _destList;
	private float[] _contextMap;
	private bool _paused = false;

	void Awake () {
		Cache();
	}

	void Start () {
		_currDest = _transform.position;
		StartCoroutine (MovementLoop());
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

	public void InitFurniturePattern (List<IFurniture> pattern) {
		_furniturePattern = pattern;
	}

	private Vector3 RaycastOrigin {
		get { return _transform.position + Vector3.up; }
	}

	void FixedUpdate () {
		if (_paused) return;
		UpdateBehaviors();
		Vector3 inputVector = ChooseDirectionGivenContextMapAndHeading(_contextMap, _transform.forward);
		if (_idle) inputVector = Vector3.zero;
		_characterMotor.AddInputWithPriority(inputVector, ControlPriority.NPC);
	}

	private bool _idle = false;
	private IEnumerator MovementLoop () {
		_idle = true;
		yield return new WaitForSeconds(Random.Range(1f,3f));
		_idle = false;
		while (true) {
			yield return StartCoroutine(PickDest());
			yield return StartCoroutine(ContinuouslySteerToDest ());
		}
	}

	// IFurniture _nextFurniture = null;
	private IEnumerator PickDest () {
		if (_destList == null || _destList.Count == 0) {
			if (_destList != null) {
				_idle = true;
				yield return new WaitForSeconds(_pauseCurve.Evaluate(Random.Range(0f,1f)));
				_idle = false;
			}

			IFurniture nextFurniture = _furniturePattern[_currFurnitureIndex];
			// _nextFurniture = nextFurniture;
			_destList = RoomManager.g.PathToFurnitureFrom(nextFurniture, _transform.position);
			_currFurnitureIndex++;
			_currFurnitureIndex %= _furniturePattern.Count;
		}

		if (_destList.Count > 0) {
			_currDest = _destList[0];
			_destList.RemoveAt(0);
		} else {
			_currDest = _transform.position;
		}
		yield break;
	}

	private IEnumerator ContinuouslySteerToDest () {
		YieldInstruction wait = new WaitForFixedUpdate();
		// float stuckSensitivity = 1f;
		// float stuckTimer = 0f;
		// float timeTillStuck = 1f;
		// Vector3 stuckDest = Vector3.zero;
		// Vector3 lastPos = _transform.position;
		do {
			Debug.DrawLine(_transform.position, _currDest);
			// if ((lastPos - _transform.position).sqrMagnitude > stuckSensitivity * _characterMotor.MovementSpeed || _paused) {
			// 	lastPos = _transform.position;
			// 	stuckTimer = 0f;
			// } else {
			// 	stuckTimer += Time.fixedDeltaTime;
			// 	if (stuckTimer >= timeTillStuck) {
			// 		stuckTimer = 0f;
			// 		if (_nextFurniture != null) {
			// 			_destList = RoomManager.g.PathToFurnitureFrom(_nextFurniture, _transform.position);
			// 		}
			// 		// stuckDest = _currDest;
			// 		// _currDest = _transform.position + (_transform.position - _currDest);
			// 		// yield return new WaitForSeconds(1f);
			// 		// _currDest = stuckDest;
			// 	}
			// }

			yield return wait;
		} while ((_currDest - _transform.position).sqrMagnitude > _destinationSensitivity);
		_currDest = _transform.position;
		yield break;
	}

	private void UpdateBehaviors () {
		if (_contextMap == null) {
			_contextMap = new float[_contextMapLOD];
		}
		System.Array.Clear(_contextMap, 0, _contextMap.Length);

		// if have goal
		GenericEvaluators.ConstantEvaluator(_contextMap, CombiningMethod.Set, value: 0.5f);
		GenericEvaluators.TargetEvaluator(_contextMap, CombiningMethod.KeepLargest, origin: RaycastOrigin, target: _currDest);
		ContextMap.VisualizeMap (_contextMap, RaycastOrigin + Vector3.up * 5f, Color.green);

		ContextMap.NormalizeMap(_contextMap);
		// ContextMap.VisualizeMap (_contextMap, RaycastOrigin + Vector3.up * 1f, Color.yellow);

		RaycastBlockEvaluator(_contextMap, CombiningMethod.Multiply,
		                      origin: RaycastOrigin, radius: _collisionAvoidanceRadius);

		float[] testMap = new float[_contextMapLOD];
		RaycastBlockEvaluator(testMap, CombiningMethod.Set,
		                      origin: RaycastOrigin, radius: _collisionAvoidanceRadius);
		// ContextMap.NormalizeMap(testMap);
		ContextMap.VisualizeMap (testMap, RaycastOrigin + Vector3.up * 2f, Color.red, scale: _collisionAvoidanceRadius);
	}

	private void RaycastBlockEvaluator (float[] mapToModify, CombiningMethod combiningMethod,
	                                    Vector3 origin, float radius) {
		int count = mapToModify.Length;

		Vector3[] directions = ContextMap.GetDiscreteUnitCircleDirections(count);

		RaycastHit hit;
		for (int i = 0; i < count; i++) {
			bool didHit = Physics.Raycast(origin, directions [i], out hit, radius);
			float hitFraction;
			if (!didHit) {
				hitFraction = 1f;
			} else {
				hitFraction = hit.distance/radius;
			}

			ContextMap.CombineMapValues(mapToModify, hitFraction * hitFraction, i, combiningMethod);
		}
	}

	private Vector3 ChooseDirectionGivenContextMapAndHeading (float[] map, Vector3 heading) {
		int count = map.Length;
		Vector3[] directions = ContextMap.GetDiscreteUnitCircleDirections (count);
		float epsilon = 0.1f;
		int bestIndex = 0;
		int i = 0;
		float maxValue = map [i];
		float bestAmountTowardDirection = Vector3.Dot(heading, directions [i]);
		for (i = 1; i < count; i++) {
			float currVal = map [i];

			// When there is a tie between directional goodness, prefer the one closest to heading.
			if (Mathf.Abs (currVal - maxValue) <= epsilon) {
				float currAmountTowardDirection = Vector3.Dot(heading, directions [i]);
				if (currAmountTowardDirection > bestAmountTowardDirection) {
					bestAmountTowardDirection = currAmountTowardDirection;
					bestIndex = i;
					maxValue = currVal;
				}
				continue;
			}

			if (currVal > maxValue) {
				bestAmountTowardDirection = Vector3.Dot(heading, directions [i]);
				bestIndex = i;
				maxValue = currVal;
				continue;
			}
		}

		Vector3 dir = directions[bestIndex] * map[bestIndex];
		dir += directions[MathUtils.Modulo(bestIndex + 1, count)] * map[MathUtils.Modulo(bestIndex + 1, count)];
		dir += directions[MathUtils.Modulo(bestIndex - 1, count)] * map[MathUtils.Modulo(bestIndex - 1, count)];
		dir /= 3f;

		return dir;
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
		if (NeedToCache) {
			Cache();
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(RaycastOrigin, _collisionAvoidanceRadius);

		// if (visualize.chosenDirection) {
			// Gizmos.color = Color.green;
			// Gizmos.DrawLine (myTransform.position, myTransform.position + (Vector3)targetDirection * 20f);
		// }

		Gizmos.color = Color.white;
		Gizmos.DrawSphere(_currDest, 0.2f);
	}
}
