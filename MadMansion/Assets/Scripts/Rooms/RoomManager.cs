using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	[SerializeField]
	private Room[] _rooms;
	public int RoomCount {
		get { return _rooms.Length; }
	}

	public static RoomManager g;

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	public List<IFurniture> RandomFurnitureList (int furnitureCount) {
		if (furnitureCount > _rooms.Length) {
			Debug.LogWarning("Can't get more furniture than rooms");
			furnitureCount = _rooms.Length;
		}
		List<IFurniture> furnitureList = new List<IFurniture>();
		HashSet<Room> _alreadyUsedRooms = new HashSet<Room>();

		for (int i = 0; i < furnitureCount; i++) {
			Room randomRoom = null;
			while (randomRoom == null || _alreadyUsedRooms.Contains(randomRoom)) { // TODO: Think about preventing infinite loops
				int randomRoomIndex = Random.Range(0,_rooms.Length);
				randomRoom = _rooms[randomRoomIndex];
			}
			_alreadyUsedRooms.Add(randomRoom);
			furnitureList.Add(randomRoom.RandomFurniture);
		}
		return furnitureList;
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

	public List<Vector3> PathToFurnitureFrom (IFurniture destFurniture, Vector3 start) {
		List<Vector3> path = new List<Vector3>();
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMesh.CalculatePath(start, destFurniture.Position, -1, navMeshPath);
		for (int i = 0; i < navMeshPath.corners.Length; i++) {
			path.Add(navMeshPath.corners[i]);
		}
		return path;
	}

	public void DimOtherRooms(Room exclude) {
		for (int i = 0; i < _rooms.Length; i++) {
			Room room = _rooms[i];
			if (room != exclude) {
				room.DimRoom();
			}
		}
	}

}