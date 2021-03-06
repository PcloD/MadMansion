﻿using UnityEngine;
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
	RectTransform _creatorsPanel;

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

	void Start () {
		_leftControlsPanel.gameObject.SetActive(false);
		_rightControlsPanel.gameObject.SetActive(false);
		StartCoroutine(SetupOffscreenPanels());
	}

	private IEnumerator SetupOffscreenPanels () {
		yield return new WaitForSeconds(0.1f);
		Vector2 desPos = new Vector2(-0.5f,0);
		Vector2 origAnchorPos = _leftControlsPanel.anchoredPosition;
		_leftControlsPanel.anchoredPosition = origAnchorPos + new Vector2(desPos.x * _leftControlsPanel.rect.width, desPos.y * _leftControlsPanel.rect.height);
		origAnchorPos = _rightControlsPanel.anchoredPosition;
		desPos = new Vector2(0.5f,0);
		_rightControlsPanel.anchoredPosition = origAnchorPos + new Vector2(desPos.x * _rightControlsPanel.rect.width, desPos.y * _rightControlsPanel.rect.height);
		_leftControlsPanel.gameObject.SetActive(true);
		_rightControlsPanel.gameObject.SetActive(true);
	}

	public void TransitionToPractice() {
		TransitionToGameMode(GameMode.Practice);
	}

	public void TransitionToPlay() {
		TransitionToGameMode(GameMode.Standard);
	}

	private void TransitionToGameMode (GameMode gameMode) {
		_playButton.interactable = false;

		StartCoroutine(FadeAndStart(_intensityTransitionDuration, gameMode));

		// Menu Panel offscreen
		StartCoroutine(MovePanelByOffset (_panel, _panelTransitionDuration, new Vector2(-1,0), _panelTransitionCurve));

		// LeftControlPanel onscreen
		StartCoroutine(MovePanelByOffset (_leftControlsPanel, _panelTransitionDuration, new Vector2(1f,0), _panelTransitionCurve));
		// RightControlPanel onscreen
		StartCoroutine(MovePanelByOffset (_rightControlsPanel, _panelTransitionDuration, new Vector2(-1f,0), _panelTransitionCurve));

		StartCoroutine(MovePanelByOffset (_creatorsPanel, _panelTransitionDuration, new Vector2(0,-1), _panelTransitionCurve));
	}

	private IEnumerator MovePanelByOffset (RectTransform panel, float timerDuration, Vector2 offset, AnimationCurve curve) {
		float timer = 0f;
		Vector2 origAnchorPos = panel.anchoredPosition;
		while (timer < timerDuration) {
			timer += Time.deltaTime;
			Vector2 desPos = curve.Evaluate(timer/timerDuration) * offset;
			panel.anchoredPosition = origAnchorPos + new Vector2(desPos.x * panel.rect.width, desPos.y * panel.rect.height);
			yield return null;
		}
		panel.anchoredPosition = origAnchorPos + new Vector2(offset.x * panel.rect.width, offset.y * panel.rect.height);
	}

	private IEnumerator FadeAndStart (float timerDuration, GameMode gameMode) {
		float timer = 0f;

		while (timer < timerDuration) {
			timer += Time.deltaTime;
			_mainLight.intensity = _dimIntensity + _lightIntensityCurve.Evaluate(timer/timerDuration) * (_playingIntensity - _dimIntensity);
			yield return null;
		}
		_mainLight.intensity = _playingIntensity;
		Events.g.Raise(new StartCharacterSelectionEvent(gameMode));
		yield return null;
	}
}
