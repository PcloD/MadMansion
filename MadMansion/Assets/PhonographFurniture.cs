using UnityEngine;
using System.Collections;

public class PhonographFurniture : MonoBehaviour, IFurniture {

    public Vector3 Position {
    	get { return transform.position; }
    }

    [SerializeField]
    private Transform _speakerTransform;
    [SerializeField]
    private Transform _crankTransform;
    [SerializeField]
    private float _crankSpeed = 1f;
    [SerializeField]
    private float _speakerDuration = 1f;
    [SerializeField]
    private float _maxSpeakerEnlargement = 0.2f;
    private bool _on = false;
    private int _visitors = 0;
    // private Quaternion _initialRotation;

    void Start() {
        // _initialRotation = _speakerTransform.rotation;
        StartCoroutine(PlaySong());
    }

    private IEnumerator PlaySong () {
        YieldInstruction wait = new WaitForFixedUpdate();
        float counter = 0f;
        while (true) {
            if (_on) {

                counter += Time.fixedDeltaTime;
                _crankTransform.Rotate(-Vector3.up * Time.fixedDeltaTime * _crankSpeed);
                _speakerTransform.localScale = new Vector3(1+Mathf.PingPong(counter/_speakerDuration,0.5f) * _maxSpeakerEnlargement * 2f,
                                                           1f,
                                                           1+Mathf.PingPong(counter/_speakerDuration,0.5f) * _maxSpeakerEnlargement * 2f);
                Debug.Log(_speakerTransform.localScale);
                counter %= _speakerDuration;
            }
            yield return wait;
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