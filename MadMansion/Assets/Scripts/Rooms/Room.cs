using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(BoxCollider))]
public class Room : MonoBehaviour {

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
		get { return new Vector3(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x), 0f,
						         Random.Range(_collider.bounds.min.y, _collider.bounds.max.y)); }
	}
}