using UnityEngine;
using System.Collections;

public class HauntUI : MonoBehaviour {

	[SerializeField]
	private GameObject _hauntIconPrefab;
	[SerializeField]
	private float _separation = 1f;

	private Transform _transform;

	private int _displayedHauntCount = 0;
	private HauntIcon[] _hauntIcons;

	void Awake () {
		_transform = transform;
	}

	void Start () {
		_hauntIcons = new HauntIcon[HauntManager.g.RequiredHauntCount];
		for (int i = 0; i < HauntManager.g.RequiredHauntCount; i++) {
			Vector3 pos = _transform.position + _transform.right * i * _separation;
			GameObject newHauntIcon = Instantiate(_hauntIconPrefab, pos, _transform.rotation) as GameObject;
			_hauntIcons[i] = newHauntIcon.GetComponent<HauntIcon>();
		}
	}

	void Update () {
		UpdateDisplayedHaunts();
		UpdateHauntTimer();
	}

	private void UpdateHauntTimer () {
		_hauntIcons[_displayedHauntCount].PercentageFilled = HauntManager.g.HauntTimerPercentage;
	}

	private void UpdateDisplayedHaunts () {
		while (_displayedHauntCount < HauntManager.g.HauntCount && _displayedHauntCount < HauntManager.g.RequiredHauntCount) {
			_hauntIcons[_displayedHauntCount].HauntState = HauntState.Haunted;
			_displayedHauntCount++;
		}

		while (_displayedHauntCount > HauntManager.g.HauntCount) {
			_hauntIcons[_displayedHauntCount-1].HauntState = HauntState.WaitForHaunt;
			_displayedHauntCount--;
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + transform.right * _separation, 0.3f);
	}
}
