using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

	[SerializeField]
	private float _destinationSensitivity = 0.5f;

	private Vector3 _currDest;
	private Transform _transform;
	private CharacterMotor _characterMotor;

	void Awake () {
		Cache();
	}

	void Start () {
		StartCoroutine (MovementLoop());
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
		_currDest = new Vector3(Random.Range(-10f,10f), 0f, Random.Range(-6.7f,6.7f)); // TODO: Make pos be in des room
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
			float completion = inputVector.sqrMagnitude/initialVector.sqrMagnitude;
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
	}

	private bool NeedToCache {
		get { return (_transform == null || _characterMotor == null); }
	}

	void OnDrawGizmos () {
		if (!this.enabled) {
			return;
		}
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(_currDest, 0.2f);
	}
}
