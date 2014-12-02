using UnityEngine;
using System.Collections;

public class FridgeFurniture : MonoBehaviour, IFurniture {

    public Vector3 Position {
    	get { return transform.position; }
    }

    [SerializeField]
    private Transform _doorTransform;
    [SerializeField]
    private float _duration = 1f;
    private bool _open = false;
    private int _visitors = 0;
    private Quaternion _initialRotation;
    void Start () {
        _initialRotation = _doorTransform.rotation;
    }

    void OnTriggerEnter (Collider other) {
        CharacterMotor character = other.GetComponent<CharacterMotor>();
        if (character != null) {
            Debug.Log("OPEN");
            _visitors++;
            if (!_open && _visitors == 1) {
                Open();
            }
        }
    }

    void OnTriggerExit (Collider other) {
        CharacterMotor character = other.GetComponent<CharacterMotor>();
        if (character != null) {
            _visitors--;
            if (_open && _visitors == 0) {
                Close();
            }
        }
    }

    private void Open () {
        _open = true;
        StartCoroutine(BeginOpening());
    }

    private void Close () {
        _open = false;
        StartCoroutine(BeginClosing());
    }

    private IEnumerator BeginOpening () {
        Quaternion open = Quaternion.Euler(0, 80, 0);
        YieldInstruction wait = new WaitForFixedUpdate();
        float counter = 0f;
        while (counter < _duration) {
            counter += Time.fixedDeltaTime;
            _doorTransform.rotation = Quaternion.Lerp(_initialRotation, open, counter/_duration);
            yield return wait;
        }
    }

    private IEnumerator BeginClosing () {
        Quaternion openRotation = Quaternion.Euler(0, 80, 0);
        YieldInstruction wait = new WaitForFixedUpdate();
        float counter = 0f;
        while (counter < _duration) {
            counter += Time.fixedDeltaTime;
            _doorTransform.rotation = Quaternion.Lerp(openRotation, _initialRotation, counter/_duration);
            yield return wait;
        }
    }

}