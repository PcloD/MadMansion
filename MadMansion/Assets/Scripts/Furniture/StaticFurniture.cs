using UnityEngine;
using System.Collections;

public class StaticFurniture : MonoBehaviour, IFurniture {
	private Transform _transform;
	public Vector3 Position {
		get { return _transform.position; }
	}

	void Awake () {
		_transform = transform;
	}
}
