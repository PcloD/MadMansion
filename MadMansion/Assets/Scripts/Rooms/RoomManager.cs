using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	[SerializeField]
	private Room[] _rooms;

	public static RoomManager g;

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	public List<Room> RandomRoomList (int roomCount) {
		List<Room> roomList = new List<Room>();
		for (int i = 0; i < roomCount; i++) {
			roomList.Add(_rooms[Random.Range(0,_rooms.Length)]);
		}
		return roomList;
	}

	public List<Vector3> PathBetweenRooms (Room room1, Room room2) {
		List<Vector3> path = new List<Vector3>();

		if (room1 == room2) {
			path.Add(room2.RandomPoint);
			return path;
		}

		RoomTransition transitionToRoom2 = room1.TransitionToRoom(room2);
		if (transitionToRoom2 == null) {
			Debug.LogError("Could not find path from " + room1.gameObject.name + " to " + room2.gameObject.name);
			return null;
		}
		path.Add(transitionToRoom2.DoorLocation);
		path.Add(room2.RandomPoint);
		return path;
	}

	public List<Vector3> PathToRoomFrom (Room destRoom, Vector3 start) {
		List<Vector3> path = new List<Vector3>();
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMesh.CalculatePath(start, destRoom.RandomPoint, -1, navMeshPath);
		for (int i = 0; i < navMeshPath.corners.Length; i++) {
			path.Add(navMeshPath.corners[i]);
		}
		return path;
	}

}