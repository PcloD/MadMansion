using UnityEngine;
using System.Collections;

public class HauntUI : MonoBehaviour {

	[SerializeField]
	private GameObject _hauntIconPrefab;
	[SerializeField]
	private float _separation = 1f;
	[SerializeField]
	private Color _chargingColor;
	[SerializeField]
	private Color _hauntingColor;

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
		if (HauntManager.g.IsHaunting) {
			_hauntIcons[_displayedHauntCount].PercentageFilled = 1f - HauntManager.g.HauntTimerPercentage;
			_hauntIcons[_displayedHauntCount].Color = _hauntingColor;
		} else {
			_hauntIcons[_displayedHauntCount].PercentageFilled = HauntManager.g.HauntChargePercentage;
			_hauntIcons[_displayedHauntCount].Color = _chargingColor;
		}

		if (HauntManager.g.CanHaunt) {
			_hauntIcons[_displayedHauntCount].HauntState = HauntState.CanHaunt;
		} else {
			_hauntIcons[_displayedHauntCount].HauntState = HauntState.WaitForHaunt;
		}
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
