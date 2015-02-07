using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CatchManager : MonoBehaviour
{

	public static CatchManager g;
	[SerializeField]
	private float
		_catchChargeDuration = 4f;
	public float CatchChargePercentage {
		get {
			return Mathf.Min (1f, _catchChargeTimer.ElapsedMilliseconds / (_catchChargeDuration * 1000f));
		}
	}

	private Stopwatch _catchChargeTimer = new Stopwatch ();
	private bool _catchChargeTimerPaused = false;
	private bool _catchChargeTimerIsRunning {
		get { return _catchChargeTimerPaused || _catchChargeTimer.IsRunning; }
	}

	void Update ()
	{

	}

	void Awake ()
	{
		if (g == null) {
			g = this;
		} else {
			Destroy (this);
		}
	}

	void OnEnable ()
	{
		Events.g.AddListener<StartGameEvent> (BeginCharging);
		Events.g.AddListener<PauseGameEvent> (PauseTimers);
		Events.g.AddListener<ResumeGameEvent> (ResumeTimers);
		Events.g.AddListener<CatchEvent> (StartCatching);
		Events.g.AddListener<FinishCatchEvent> (FinalizeCatching);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PauseGameEvent> (PauseTimers);
		Events.g.RemoveListener<ResumeGameEvent> (ResumeTimers);
		Events.g.RemoveListener<StartGameEvent> (BeginCharging);
		Events.g.RemoveListener<CatchEvent> (StartCatching);
		Events.g.RemoveListener<FinishCatchEvent> (FinalizeCatching);
	}

	private void PauseTimers (PauseGameEvent e)
	{
		if (_catchChargeTimer.IsRunning) {
			_catchChargeTimer.Stop ();

			_catchChargeTimerPaused = true;

		}
	}

	private void ResumeTimers (ResumeGameEvent e)
	{
		if (_catchChargeTimerPaused) {
			_catchChargeTimer.Start ();

			_catchChargeTimerPaused = false;
		}
	}

	private void BeginCharging (StartGameEvent e)
	{
		// Handle event here
		StartCatchCharge ();
	}

	private void StartCatching (CatchEvent e)
	{
		if (e.successful) {
			print ("Succeeded catch event");
			TimeManager.g.StartBulletTime ();
			_isCatching = true;
		} else {
			print ("Failed catch event");
		}
	}

	private void StopCatching ()
	{
		TimeManager.g.StopBulletTime ();
		_isCatching = false;
		_catchChargeTimer.Reset ();
		StartCatchCharge ();

	}

	private void FinalizeCatching (FinishCatchEvent e)
	{
		if (e.guess == null) {
			return;
		}
		GhostController ghostController = e.guess.GetComponent<GhostController> ();
		if (ghostController != null && ghostController.enabled) {
			if (e.hunter == e.guess) {
				Events.g.Raise (new EndGameEvent (winner: Player.NoPlayer, rationale: EndReason.HunterCaughtGhostInSameBody));
			} else {
				Events.g.Raise (new EndGameEvent (winner: Player.HunterPlayer, rationale: EndReason.HunterCaughtGhost));
			}
		} else {
			//Events.g.Raise(new EndGameEvent(winner: Player.GhostPlayer, rationale: EndReason.HunterCaughtInnocent));
			StopCatching ();
			Events.g.Raise (new CatchWrongEvent ());

		}
	}

	private bool _isCatching = false;
	public bool IsCatching {
		get { return _isCatching; }
	}

	private void StartCatchCharge ()
	{
		_catchChargeTimer.Start ();


	}

	public bool CanCatch {
		get { return _catchChargeTimerIsRunning && (_catchChargeTimer.ElapsedMilliseconds > _catchChargeDuration * 1000f); }
	}
}