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
		Events.g.AddListener<HauntEvent>(PlayHauntSound);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<HauntEvent>(PlayHauntSound);
	}

	private void PlayHauntSound (HauntEvent e)
	{
		if (e.IsStart) {
			_audioSource.pitch = _audioSource.clip.length/e.duration;
			_audioSource.Play();
		} else {
			_audioSource.Stop();
		}
	}

}