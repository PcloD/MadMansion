using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour {
	void Start () {
		if (!Application.isEditor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
