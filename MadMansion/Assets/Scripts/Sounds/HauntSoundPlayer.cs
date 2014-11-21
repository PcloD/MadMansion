using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class HauntSoundPlayer : MonoBehaviour {
	private AudioSource _audioSource;

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

	private void GhostSound (SmellEvent e)
	{
		// Handle event here
		if (e.IsStart) {
			Vector3 toOldGhostPos = (e.hunter.transform.position - GhostTracker.g.HistoricalLocation);
			float volumeScale = Mathf.Min(e.hunter.VolumeReduction/toOldGhostPos.magnitude, 1f);
			PlayHauntSound(volumeScale);
		} else {
			StopHauntSound();
		}
	}


	private void PlayHauntSound (float volumeScale) {
		if (!_audioSource.isPlaying) {
			_audioSource.Play();
		}
		_audioSource.volume = volumeScale;
	}

	public void StopHauntSound () {
		_audioSource.Stop();
	}

}
