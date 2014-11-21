using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class TimeManager : MonoBehaviour {

	[SerializeField]
	private AnimationCurve _bulletTimeTransition;
	[SerializeField]
	private float _introTransitionDuration = 1f;
	[SerializeField]
	private float _outroTransitionDuration = 0.3f;
	[SerializeField]
	private float _hunterBulletTimeResilience = 2f;
	private Stopwatch _stopWatch = new Stopwatch();

	private long _stopTime;

	public static TimeManager g;

	void Awake () {
		if (g == null) {
			g = this;
			Reset();
		} else {
			Destroy(this);
		}
	}

	public float TimeScale {
		get {
			float timeScale = 1f;
			if (_stopWatch.IsRunning) {
				if (_stopTime > 0) {
					// Leaving BulletTime
					float secondsElapsedSinceStop = (_stopWatch.ElapsedMilliseconds - _stopTime)/1000f;
					if (_introTransitionDuration <= 0f) {
						timeScale = 0f;
					} else {
						timeScale = _outroTransitionDuration/_introTransitionDuration * (_bulletTimeTransition.Evaluate(_introTransitionDuration - secondsElapsedSinceStop));
					}
					if (_outroTransitionDuration - secondsElapsedSinceStop <= 0f) {
						Reset();
					}
				} else {
					// Entering BulletTime
					timeScale = _bulletTimeTransition.Evaluate(_stopWatch.ElapsedMilliseconds/1000f);
				}
			}
			return timeScale;
		}
	}

	public float HunterTimeScale {
		get { return Mathf.Min(1f, TimeScale * _hunterBulletTimeResilience); }
	}

	private void Reset () {
		_stopTime = -1;
		_stopWatch.Stop();
		_stopWatch.Reset();
	}

	public void StartBulletTime () {
		_stopWatch.Start();
	}

	public void StopBulletTime () {
		_stopTime = _stopWatch.ElapsedMilliseconds;
	}

}
