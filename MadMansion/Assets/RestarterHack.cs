using UnityEngine;
using System.Collections;

public class RestarterHack : MonoBehaviour {
	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) {
			Events.g.Raise(new RestartGameEvent());
		}
	}
}
