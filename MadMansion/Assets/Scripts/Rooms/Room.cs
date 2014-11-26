using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadMansion.Extensions.Interface;

[RequireComponent (typeof(BoxCollider))]
public class Room : MonoBehaviour {

	[SerializeField]
	private Transform[] _furnitureTransform;
	private HashSet<CharacterMotor> _characters = new HashSet<CharacterMotor>();
	private BoxCollider _collider;
	private Dictionary<Room,RoomTransition> _transitionsForRooms = new Dictionary<Room,RoomTransition>();

	void Awake () {
		_collider = GetComponent<BoxCollider>();
	}

	void OnTriggerExit (Collider other) {
		CharacterMotor character = other.GetComponent<CharacterMotor>();
		if (character != null) {
			if (_characters.Contains(character)) {
				_characters.Remove(character);
			}
		}
	}

	void OnTriggerStay (Collider other) {
		CharacterMotor character = other.GetComponent<CharacterMotor>();
		if (character != null) {
			if (!_characters.Contains(character)) {
				_characters.Add(character);
			}
		}
	}

	public void AddTransition (RoomTransition transition) {
		_transitionsForRooms.Add(transition.ConnectedRoom(this), transition);
	}

	public RoomTransition TransitionToRoom (Room room) {
		if (!_transitionsForRooms.ContainsKey(room)) {
			return null;
		} else {
			return _transitionsForRooms[room];
		}
	}

	public HashSet<CharacterMotor> Characters {
		get { return _characters; }
	}

	public Vector3 RandomPoint {
		get {	float inset = 1f;
				return new Vector3(Random.Range(_collider.bounds.min.x + inset, _collider.bounds.max.x - inset), 0f,
						         Random.Range(_collider.bounds.min.z + inset, _collider.bounds.max.z - inset)); }
	}

	public IFurniture RandomFurniture {
		get {
			return _furnitureTransform[Random.Range(0,_furnitureTransform.Length)].gameObject.GetInterface<IFurniture>();
		}
	}

	public GhostController ClosestGhostToCharacter (CharacterMotor character) {
        Vector3 characterPos = character.transform.position;
        characterPos.y = 0f;
        GhostController closest = null;
        float sqrDistToClosest = Mathf.Infinity;
        foreach (CharacterMotor currNeighbor in _characters) {
        	if (currNeighbor == character) {
        		continue;
        	}
			Vector3 currNeighborPos = currNeighbor.transform.position;
			currNeighborPos.y = 0f;
			float sqrDistToCurrNeighbor = (currNeighborPos - characterPos).sqrMagnitude;
			if (closest == null || sqrDistToCurrNeighbor < sqrDistToClosest) {
            	sqrDistToClosest = sqrDistToCurrNeighbor;
            	GhostController currNeighborGhostController = currNeighbor.GetComponent<GhostController>();
            	if (currNeighborGhostController != null) {
            		closest = currNeighborGhostController.GetComponent<GhostController>();
            	}
            }
        }
        return closest;
	}
}