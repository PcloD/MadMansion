using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

public class AfterCatchEffects : MonoBehaviour
{
	//private Stopwatch _effectTimerCatchWrong = new Stopwatch ();
	[SerializeField]
	private Text
		_textForCatchWrong;
	[SerializeField]
	private Text
		_timeCounterForCatchWrong;
	[SerializeField]
	private float
		_waitingDuration = 4f;
	private float _timer;

	void OnEnable ()
	{
		Events.g.AddListener<CatchEndEvent> (StartAfterCatchEffect);
	}
	
	void OnDisable ()
	{
		Events.g.RemoveListener<CatchEndEvent> (StartAfterCatchEffect);
	}
	private void StartAfterCatchEffect (CatchEndEvent e)
	{
		if (e.catchRight) {
			
		} else {
			_textForCatchWrong.gameObject.SetActive (true);
			_timeCounterForCatchWrong.gameObject.SetActive (true);
			StartCoroutine (SecondsCounter ());
			//_effectTimerCatchWrong.Start ();


		}

	}
	private IEnumerator SecondsCounter ()
	{
		while (_timer<_waitingDuration) {
			_timer += Time.deltaTime;
			int i = (int)(_waitingDuration - _timer);
			_timeCounterForCatchWrong.text = i.ToString ();
			yield return null;
		}
		EndAfterCatchEffects ();


	}
	private void  EndAfterCatchEffects ()
	{
		_textForCatchWrong.gameObject.SetActive (false);
		_timeCounterForCatchWrong.gameObject.SetActive (false);
		//_effectTimerCatchWrong.Stop ();
		//_effectTimerCatchWrong.Reset ();
		Events.g.Raise (new CatchEffectEndEvent ());

	}
	void Update ()
	{
		//print ("_effectTimerCatchWrong" + _effectTimerCatchWrong.ElapsedMilliseconds / 1000f);
		//if (_effectTimerCatchWrong.IsRunning && _waitingDuration > _effectTimerCatchWrong.ElapsedMilliseconds / 1000f) {
		//	Events.g.Raise (new CatchEffectEnd ());

		//}
	}
}
