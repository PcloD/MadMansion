using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CatchManager : MonoBehaviour {

	public static CatchManager g;
	[SerializeField]
	private float _catchChargeDuration = 4f;
	public float PossessionChargePercentage {
		get {
			return Mathf.Min(1f, _catchChargeTimer.ElapsedMilliseconds/(_catchChargeDuration * 1000f));
		}
	}

	private Stopwatch _catchChargeTimer = new Stopwatch();

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}

	public void StartCatchCharge () {
		_catchChargeTimer.Start();
	}

	public bool CanCatch {
		get { return _catchChargeTimer.IsRunning && (_catchChargeTimer.ElapsedMilliseconds > _catchChargeDuration * 1000f); }
	}
}