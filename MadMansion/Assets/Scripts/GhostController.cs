using UnityEngine;
using System.Collections;
using InControl;

public class GhostController : MonoBehaviour {
	[SerializeField]
	private float _possessionRadius = 5f;

	private Transform _transform;
	private CharacterMotor _characterMotor;

	private CurrRoomFinder _currRoomFinder;

	void Awake () {
		Cache();
	}

	void Update () {
		HandleInput(PlayerInputManager.g.Ghost);
	}

	void FixedUpdate () {
		GhostTracker.g.RecordLocation(_transform.position);
	}

	private void HandleInput (InputDevice device) {
		if (device == null) {
			return;
		}
		Vector3 inputVector = new Vector3(device.LeftStickX.Value, 0f, device.LeftStickY.Value);
		_characterMotor.AddInputWithPriority(inputVector, ControlPriority.Ghost);

		InputControl possessionButton = device.Action1;
		InputControl hauntButton = device.Action3;
		if (possessionButton.WasPressed) {
			JumpToClosest();
		}
		if (hauntButton.WasPressed) {
			HauntManager.g.StartHauntInRoom(_currRoomFinder.Room);
		}
	}

	private void JumpToClosest () {
        Collider[] neighborColliders = Physics.OverlapSphere(_transform.position, _possessionRadius);
        GhostController closest = null;
        float sqrDistToClosest = Mathf.Infinity;
        int neighborCount = neighborColliders.Length;
        for (int i = 0; i < neighborCount; i++) {
        	GameObject currNeighbor = neighborColliders[i].gameObject;
        	GhostController currNeighborGhostController = currNeighbor.GetComponent<GhostController>();
        	if (currNeighborGhostController == null || currNeighborGhostController == this) {
        		continue;
        	}
        	Vector3 currNeighborPos = currNeighbor.transform.position;
        	float sqrDistToCurrNeighbor = (currNeighborPos - _transform.position).sqrMagnitude;
            if (closest == null || sqrDistToCurrNeighbor < sqrDistToClosest) {
            	sqrDistToClosest = sqrDistToCurrNeighbor;
	            closest = currNeighbor.GetComponent<GhostController>();
            }
        }
        if (closest != null) {
        	Debug.Log("Jump!");
        	closest.enabled = true;
        	this.enabled = false;
    	} else {
    		Debug.Log("Can't Jump!");
    	}
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
		if (NeedToCache) {
			Cache();
		}
		if (!this.enabled) {
			return;
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere (_transform.position, _possessionRadius);
	}
}