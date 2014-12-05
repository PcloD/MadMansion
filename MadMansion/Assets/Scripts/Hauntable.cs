using UnityEngine;
using System.Collections;

public class Hauntable : MonoBehaviour {

	private ParticleSystem _hauntParticles;

	void Awake () {
		_hauntParticles = GetComponent<ParticleSystem>();
	}

	void Start () {
		StopHaunting();
	}

	public void StartHaunting () {
		_hauntParticles.Play();
	}

	public void StopHaunting () {
		_hauntParticles.Stop();
		_hauntParticles.Clear();
	}
}
