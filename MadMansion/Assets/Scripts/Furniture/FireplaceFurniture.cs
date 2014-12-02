using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireplaceFurniture : MonoBehaviour, IFurniture {

    public Vector3 Position {
    	get { return transform.position; }
    }

    [SerializeField]
    private GameObject _fireParticlePrefab;
    [SerializeField]
    private Transform _fireSourceTransform;
    [SerializeField]
    private int _maxParticles = 100;
    [SerializeField]
    private float _maxTimeTillEmission = 1f;
    [SerializeField]
    private float _particleSpeed = 1f;
    [SerializeField]
    private float _particleAcceleration = 1f;
    [SerializeField]
    private float _particleLifetime = 1f;
    [SerializeField]
    private Vector2 _maxFireRange;
    [SerializeField]
    AnimationCurve _particleSizeCurve;
    private HashSet<FireParticle> visibleParticles = new HashSet<FireParticle>();
    private Queue<FireParticle> pooledParticles = new Queue<FireParticle>();
    private bool _on = true;//false;
    private int _visitors = 0;

    void Start() {
        // _initialRotation = _speakerTransform.rotation;
        StartCoroutine(EmitFlames());
    }

    private IEnumerator EmitFlames () {
        YieldInstruction wait = new WaitForFixedUpdate();
        float counter = 0;
        while (true) {
            if (_on) {
                counter -= Time.fixedDeltaTime;
                if (counter <= 0) {
                    EmitParticles();
                    counter = Random.Range(0f,_maxTimeTillEmission);
                }
            }
            SimulateParticles();
            yield return wait;
        }

    }

    private void SimulateParticles () {
        List<FireParticle> elementsToRemove = new List<FireParticle>();
        foreach (FireParticle p in visibleParticles) {
            if (p.NeedsRecycling) {
                elementsToRemove.Add(p);
            }
        }
        for (int i = 0; i < elementsToRemove.Count; i++) {
            FireParticle p = elementsToRemove[i];
            visibleParticles.Remove(p);
            p.gameObject.SetActive(false);
            pooledParticles.Enqueue(p);
        }
    }

    private void EmitParticles () {
        FireParticle unusedParticle = null;
        Vector3 randomPos = new Vector3(Random.Range(-_maxFireRange.x,_maxFireRange.x), 0,Random.Range(-_maxFireRange.y,_maxFireRange.y)) + _fireSourceTransform.position;
        if (pooledParticles.Count + visibleParticles.Count < _maxParticles) {
            GameObject newFireParticleObject = Instantiate(_fireParticlePrefab, randomPos, Quaternion.identity) as GameObject;
            newFireParticleObject.transform.SetParent(_fireSourceTransform);
            unusedParticle = newFireParticleObject.GetComponent<FireParticle>();
        } else if (pooledParticles.Count > 0) {
            unusedParticle = pooledParticles.Dequeue();
            unusedParticle.gameObject.SetActive(true);
            unusedParticle.gameObject.transform.position = randomPos;
        }
        if (unusedParticle != null) {
            visibleParticles.Add(unusedParticle);
            unusedParticle.Init(_particleLifetime, _particleSpeed, _particleAcceleration, _particleSizeCurve);
        }
    }

    void OnTriggerEnter (Collider other) {
        CharacterMotor character = other.GetComponent<CharacterMotor>();
        if (character != null) {
            _visitors++;
            if (!_on && _visitors == 1) {
                On();
            }
        }
    }

    void OnTriggerExit (Collider other) {
        CharacterMotor character = other.GetComponent<CharacterMotor>();
        if (character != null) {
            _visitors--;
            if (_on && _visitors == 0) {
                Off();
            }
        }
    }

    private void On () {
        _on = true;
    }

    private void Off () {
        _on = false;
    }

}