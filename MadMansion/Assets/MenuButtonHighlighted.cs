using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButtonHighlighted : MonoBehaviour {
	private Button _button;
	private Vector3 _defaultScale;
	void Awake () {
		_button = GetComponent<Button>();
		_defaultScale = _button.transform.localScale;
	}

	public void Highlight () {
		_button.transform.localScale = _defaultScale * 1.2f;
	}

	public void UnHighlight () {
		_button.transform.localScale = _defaultScale;
	}
}