using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour {
	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}
