using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayTransition : MonoBehaviour {

	[SerializeField]
	Light _mainLight;
	[SerializeField]
	float _dimIntensity = 0f;
	[SerializeField]
	float _playingIntensity = 0.09f;
	[SerializeField]
	float _intensityTransitionDuration = 1f;
	Button _playButton;
	[SerializeField]
	RectTransform _panel;

	[SerializeField]
	RectTransform _leftControlsPanel;
	[SerializeField]
	RectTransform _rightControlsPanel;

	[SerializeField]
	AnimationCurve _lightIntensityCurve;
	[SerializeField]
	AnimationCurve _panelTransitionCurve;
	[SerializeField]
	float _panelTransitionDuration = 0.2f;


	void Awake () {
		_playButton = GetComponent<Button>();
		_mainLight.intensity = _dimIntensity;
	}

	public void TransitionToPlay() {
		Debug.Log("Play Clicked");
		_playButton.interactable = false;
		StartCoroutine(FadeAndStart(_intensityTransitionDuration));
		StartCoroutine(MoveMenuPanelOffscreen(_panelTransitionDuration));

		// StartCoroutine(MoveHunterPanelOnscreen(_panelTransitionDuration));
		// StartCoroutine(MoveGhostPanelOnscreen(_panelTransitionDuration));
	}

	private IEnumerator MoveMenuPanelOffscreen (float timerDuration) {
		float timer = 0f;
		Vector2 destOffset = new Vector2(-1,0);
		Vector2 origAnchorPos = _panel.anchoredPosition;
		while (timer < timerDuration) {
			timer += Time.deltaTime;
			Vector2 desPos = _panelTransitionCurve.Evaluate(timer/timerDuration) * destOffset;
			_panel.anchoredPosition = origAnchorPos + new Vector2(desPos.x * _panel.rect.width, desPos.y * _panel.rect.height);
			yield return null;
		}
		_panel.anchoredPosition = origAnchorPos + new Vector2(destOffset.x * _panel.rect.width, destOffset.y * _panel.rect.height);
	}

	// private IEnumerator MoveGhostPanelOnscreen (float timerDuration) {
	// 	float timer = 0f;
	// 	Vector2 destOffset = new Vector2(-1,0);
	// 	Vector2 origAnchorPos = _panel.anchoredPosition;
	// 	while (timer < timerDuration) {
	// 		timer += Time.deltaTime;
	// 		Vector2 desPos = _panelTransitionCurve.Evaluate(timer/timerDuration) * destOffset;
	// 		_panel.anchoredPosition = origAnchorPos + new Vector2(desPos.x * _panel.rect.width, desPos.y * _panel.rect.height);
	// 		yield return null;
	// 	}
	// 	_panel.anchoredPosition = origAnchorPos + new Vector2(destOffset.x * _panel.rect.width, destOffset.y * _panel.rect.height);
	// }

	private IEnumerator FadeAndStart (float timerDuration) {
		float timer = 0f;

		while (timer < timerDuration) {
			timer += Time.deltaTime;
			_mainLight.intensity = _dimIntensity + _lightIntensityCurve.Evaluate(timer/timerDuration) * (_playingIntensity - _dimIntensity);
			yield return null;
		}
		_mainLight.intensity = _playingIntensity;
		Events.g.Raise(new StartCharacterSelectionEvent());
		yield return null;
	}
}
