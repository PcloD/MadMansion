using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HauntUI : MonoBehaviour {

	[SerializeField]
	private GameObject _hauntIconPrefab;
	[SerializeField]
	private float _separation = 1f;
	[SerializeField]
	private Renderer _radialProgressRenderer;

	private int _displayedHauntCount;
	private List<GameObject> _hauntIcons = new List<GameObject>();
	private Transform _currTransform;

	void Start () {
		_currTransform = transform;
	}

	void Update () {
		UpdateDisplayedHaunts();
		UpdateRadialTimer();
	}

	private void UpdateRadialTimer () {
		_radialProgressRenderer.material.SetFloat ("_Cutoff", 1f - HauntManager.g.HauntTimerPercentage);
	}

	private void UpdateDisplayedHaunts () {
		while (_displayedHauntCount < HauntManager.g.HauntCount) {
			Vector3 pos = _currTransform.position + transform.right * _separation;
			GameObject newHauntIcon = Instantiate(_hauntIconPrefab, pos, _currTransform.rotation) as GameObject;
			_currTransform = newHauntIcon.transform;
			_hauntIcons.Add(newHauntIcon);
			_displayedHauntCount++;
		}
		while (_displayedHauntCount > HauntManager.g.HauntCount) {
			Destroy(_hauntIcons[_displayedHauntCount - 1]);
			_hauntIcons.RemoveAt(_displayedHauntCount - 1);
			_displayedHauntCount--;
			if (_displayedHauntCount > 0) {
				_currTransform = _hauntIcons[_displayedHauntCount - 1].transform;
			} else {
				_currTransform = transform;
			}
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + transform.right * _separation, 0.3f);
	}
}
