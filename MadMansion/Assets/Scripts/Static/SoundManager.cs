using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	public static SoundManager g;

	[SerializeField]
	private AudioSource _ghostSoundSource;

	void Awake () {
		if (g == null) {
			g = this;
		} else {
			Destroy(this);
		}
	}


	public void PlayGhostSound (float volumeScale) {
		if (!_ghostSoundSource.isPlaying) {
			_ghostSoundSource.Play();
		}
		_ghostSoundSource.volume = volumeScale;
	}

	public void StopGhostSound () {
		_ghostSoundSource.Stop();
	}

}
