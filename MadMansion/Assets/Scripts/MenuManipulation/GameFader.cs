using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFader : MonoBehaviour {

	[SerializeField]
	float _timeToFadeIn = 2f;
	[SerializeField]
	float _timeToFadeOut = 1f;
	[SerializeField]
	AnimationCurve _fadeCurve;

	private Image _image;

	void Awake () {
		_image = GetComponent<Image>();
	}

	void OnEnable () {
		Events.g.AddListener<RestartGameEvent>(RestartGame);
	}

	void OnDisable () {
		Events.g.RemoveListener<RestartGameEvent>(RestartGame);
	}

	void Start () {
		StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
	}

	private IEnumerator Fade (float timerDuration, AnimationCurve curve, bool fadeIn) {
		Color startColor = _image.color;
		Color newColor;
		float alpha;
		float timer = 0f;
		while (timer < timerDuration) {
			Debug.Log(timer);
			timer += Time.deltaTime;
			if (fadeIn) {
				alpha = curve.Evaluate(timer/timerDuration);
			} else {
				alpha = 1f-curve.Evaluate(timer/timerDuration);
			}
			newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
			_image.color = newColor;
			yield return null;
		}
		if (fadeIn) {
			alpha = curve.Evaluate(1f);
		} else {
			alpha = 1f-curve.Evaluate(1f);
		}
		newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
		_image.color = newColor;
	}

	IEnumerator FadeOutAndRestart (float timerDuration, AnimationCurve curve) {
		yield return StartCoroutine(Fade(timerDuration, curve, fadeIn: false));
		Application.LoadLevel (Application.loadedLevel); // TODO: Make this nicer
	}

	private void RestartGame (RestartGameEvent e) {
		StartCoroutine(FadeOutAndRestart(_timeToFadeOut, _fadeCurve));
	}
}
