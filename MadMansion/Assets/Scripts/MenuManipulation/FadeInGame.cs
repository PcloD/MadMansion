using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInGame : MonoBehaviour {

	[SerializeField]
	float _timeToFadeIn = 0.1f;
	[SerializeField]
	AnimationCurve _fadeCurve;

	private Image _image;

	void Awake () {
		_image = GetComponent<Image>();
	}

	void Start () {
		StartCoroutine(FadeIn(_timeToFadeIn, _fadeCurve));
	}

	private IEnumerator FadeIn (float timerDuration, AnimationCurve curve) {
		Color startColor = _image.color;
		Color newColor;
		float alpha;
		float timer = 0f;
		while (timer < timerDuration) {
			timer += Time.deltaTime;
			alpha = curve.Evaluate(timer/timerDuration);
			newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
			_image.color = newColor;
			yield return null;
		}
		alpha = curve.Evaluate(1f);
		newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
		_image.color = newColor;
	}
}
