using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButtonHighlighted : MonoBehaviour {
	private Button _button;
	private Vector3 _defaultScale;
	private PulseComponent _pulser;
	void Awake () {
		_button = GetComponent<Button>();
		_defaultScale = _button.transform.localScale;
		_pulser = GetComponent<PulseComponent>();
		if(_pulser != null) {
			_pulser.enabled = false;
		}
	}

	public void Highlight () {
		// _button.transform.localScale = _defaultScale * 1.2f;
		if(_pulser != null) {
			_pulser.enabled = true;
		}
	}

	public void UnHighlight () {
		// _button.transform.localScale = _defaultScale;
		if(_pulser != null) {
			_pulser.enabled = false;
		}
	}

}