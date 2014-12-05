using UnityEngine;
using System.Collections;

public class SetScreenRes : MonoBehaviour {
	[SerializeField]
	private int width = 1920;
	[SerializeField]
	private int height = 1080;
	[SerializeField]
	private bool fs = true;

	void Start () {
		Screen.SetResolution(width, height, fs);
	}

}
