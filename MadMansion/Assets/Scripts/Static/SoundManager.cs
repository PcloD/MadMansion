using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	public static SoundManager g;

	[SerializeField]
	private AudioClip _ghostSound;

	private AudioSource _audioSource;

	void Awake () {
		if (g == null) {
			g = this;
			_audioSource = GetComponent<AudioSource>();
		} else {
			Destroy(this);
		}
	}


	public void PlayGhostSound (float volumeScale) {
		if (!_audioSource.isPlaying) {
			_audioSource.Play();
			_audioSource.clip = _ghostSound;
		}
		_audioSource.volume = volumeScale;
	}

	public void StopGhostSound () {
		_audioSource.Stop();
	}

}
