using UnityEngine;
using System.Collections;

public class HauntManager : MonoBehaviour {

	public static HauntManager g;
	[SerializeField]
	private float _hauntCooldown = 4f;
	[SerializeField]
	private float _hauntDuration = 4f;
	[SerializeField] // XXX: TODO: make this implicit
	private float _hauntTimerPercentage = 0.5f;
	public float HauntTimerPercentage {
		get { return _hauntTimerPercentage; }
	}
	[SerializeField]
	private int _hauntCount = 4;
	public int HauntCount {
		get { return _hauntCount; }
	}

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

}
