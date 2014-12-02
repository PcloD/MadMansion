using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireParticle : MonoBehaviour {
	[SerializeField]
	private GameObject _fireParticlePrefab;
	private ParticleSystem _particleSystem;
	private ParticleSystem.Particle[] _allParticles = new ParticleSystem.Particle[100];
	private Dictionary<ParticleSystem.Particle,Transform> _particleMap = new Dictionary<ParticleSystem.Particle,Transform>();
	// Use this for initialization
	void Start () {

		//var FireParticleSystem=this.GetComponent<Particle System>;
	}
	void Awake(){
		_particleSystem=GetComponent<ParticleSystem>();
	}


	// Update is called once per frame
	void Update () {
		//int num=ParticleSystem.getParticles(FireParticleSystem);

		int particleCount = _particleSystem.GetParticles(_allParticles);
		
		for(int index = 0; index < particleCount; index++){
			ParticleSystem.Particle p = _allParticles[index];

			if (!_particleMap.ContainsKey(p))
			{
				GameObject newFireParticle = Instantiate(_fireParticlePrefab, p.position, Quaternion.identity) as GameObject;
				_particleMap[p] = newFireParticle.transform;
			}
			else
			{
				_particleMap[p].position = p.position;
			}
			//work your magic
		}
	
	}

}
