using UnityEngine;
using System.Collections;

public class CurrRoomFinder : MonoBehaviour {
	private Room _currentRoom = null;
	public Room Room {
		get { return _currentRoom; }
	}

	void OnTriggerStay (Collider other) {
		Room room = other.GetComponent<Room>();
		if (room != null) {
			_currentRoom = room;
		}
	}
}
