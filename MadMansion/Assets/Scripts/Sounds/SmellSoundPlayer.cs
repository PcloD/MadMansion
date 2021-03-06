﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class SmellSoundPlayer : MonoBehaviour {
	private AudioSource _audioSource;
	[SerializeField]
	private bool _smellingIsBinary = true;
	[SerializeField]
	private AudioClip _ghostPresentSoundClip;
	[SerializeField]
	private AudioClip _noGhostSoundClip;
	[SerializeField]
	private AudioClip _notChargedSoundClip;

	void Awake () {
		_audioSource = GetComponent<AudioSource>();
	}

	void OnEnable ()
	{
		Events.g.AddListener<SmellEvent>(GhostSound);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<SmellEvent>(GhostSound);
	}

	private HunterController _hunter;
	private bool _playing = false;
	private void GhostSound (SmellEvent e)
	{
		// Handle event here
		if (e.IsStart) {
			_hunter = e.hunter;
			if (_smellingIsBinary) {
				if (e.room == GhostTracker.g.CurrGhostRoom) {
					_audioSource.clip = _ghostPresentSoundClip;
				} else {
					_audioSource.clip = _noGhostSoundClip;
				}
			}
			_playing = true;
		} else {
			_playing = false;
			StopSmellSound();
		}
	}

	private void Update () {
		if (_playing) {
			Vector3 toOldGhostPos = (_hunter.transform.position - GhostTracker.g.HistoricalLocation);
			float volumeScale = Mathf.Min(_hunter.VolumeReduction/toOldGhostPos.magnitude, 1f);
			PlaySmellSound(volumeScale);
		}
	}

	private void PlaySmellSound (float volumeScale) {
		if (!_audioSource.isPlaying) {
			_audioSource.loop = true;
			_audioSource.Play();
		}
		if (!_smellingIsBinary) {
			_audioSource.volume = volumeScale;
			_audioSource.pitch = Mathf.Max(1f-volumeScale, 0.5f);
		}
	}

	public void StopSmellSound () {
		_audioSource.Stop();
	}

}
