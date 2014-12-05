using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class PossessionSoundPlayer : MonoBehaviour {
	[SerializeField]
	private AudioClip _successSound;
	[SerializeField]
	private AudioClip _failureSound;
	private AudioSource _audioSource;

	void Awake () {
		_audioSource = GetComponent<AudioSource>();
	}

	void OnEnable ()
	{
		Events.g.AddListener<PossessionEvent>(PossessionSound);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<PossessionEvent>(PossessionSound);
	}

	private void PossessionSound (PossessionEvent e)
	{
		if (e.succeeded) {
			PlayPossessionSound();
		} else {
			PlayFailedPossessionSound();
		}
	}


	private void PlayPossessionSound () {
		_audioSource.clip = _successSound;
		_audioSource.Play();
		Debug.Log("JUMP");
	}

	public void PlayFailedPossessionSound () {
		_audioSource.clip = _failureSound;
		_audioSource.Play();
		Debug.Log("FAILED JUMP");
	}

}
