using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RoomTransition : MonoBehaviour {

	[SerializeField]
	private Room _room1;
	[SerializeField]
	private Room _room2;

	private Transform _transform;

	public Vector3 DoorLocation {
		get { return _transform.position; }
	}

	private Dictionary<Room,Room> _connections = null;
	public Room ConnectedRoom (Room sourceRoom) {
		if (_connections == null) {
			_connections = new Dictionary<Room,Room>();
			_connections.Add(_room1, _room2);
			_connections.Add(_room2, _room1);
		}
		if (!_connections.ContainsKey(sourceRoom)) {
			return null;
		}
		return _connections[sourceRoom];
	}

	void Awake () {
		_transform = transform;
		_room1.AddTransition(this);
		_room2.AddTransition(this);
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireSphere(transform.position, 0.2f);
		Gizmos.DrawLine(_room1.transform.position, transform.position);
		Gizmos.DrawLine(_room2.transform.position, transform.position);
	}
}