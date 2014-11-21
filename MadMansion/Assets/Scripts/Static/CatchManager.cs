using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CatchManager : MonoBehaviour {

	public static CatchManager g;
	[SerializeField]
	private float _catchChargeDuration = 4f;
	public float CatchChargePercentage {
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

	void OnEnable ()
	{
		Events.g.AddListener<StartGameEvent>(BeginCharging);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<StartGameEvent>(BeginCharging);
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartCatchCharge();
	}

	private bool _isCatching = false;
	public bool IsCatching {
		get { return _isCatching; }
		set { _isCatching = value; } // XXX: Tight coupling
	}

	private void StartCatchCharge () {
		_catchChargeTimer.Start();
	}

	public bool CanCatch {
		get { return _catchChargeTimer.IsRunning && (_catchChargeTimer.ElapsedMilliseconds > _catchChargeDuration * 1000f); }
	}
}