using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CatchState {
	WaitForCatch,
	CanCatch,
	IsCatching
}

public class CatchUI : MonoBehaviour {

	[SerializeField]
	private Sprite _waitForCatchImage;
	[SerializeField]
	private Sprite _canCatchImage;
	[SerializeField]
	private Sprite _isCatchingImage;
	[SerializeField]
	private Image _radialTimerRenderer;

	[SerializeField]
	private Color _chargingColor;
	[SerializeField]
	private Color _isCatchingColor;

	private CatchState _possessionState;
	public CatchState CatchState {
		set {
			_possessionState = value;
			switch (_possessionState) {
				case CatchState.CanCatch:
					_spriteRenderer.sprite = _canCatchImage;
					break;
				case CatchState.IsCatching:
					_spriteRenderer.sprite = _isCatchingImage;
					break;
				default:
					_spriteRenderer.sprite = _waitForCatchImage;
					break;
			}
		}
	}

	private Image _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<Image>();
		_radialTimerRenderer.material = new Material(_radialTimerRenderer.material);
	}

	void Start () {
		CatchState = CatchState.WaitForCatch;
		_radialTimerRenderer.material.SetFloat ("_Fraction", 1f);
	}

	void Update () {
		UpdateCatchTimer();
	}

	public float PercentageFilled {
		set { _radialTimerRenderer.material.SetFloat ("_Fraction", value); }
	}

	public Color Color {
		set { _radialTimerRenderer.material.SetColor ("_Color", value); }
	}

	private void UpdateCatchTimer () {
		if (CatchManager.g.IsCatching) {
			CatchState = CatchState.IsCatching;
			PercentageFilled = 1f;
			Color = _isCatchingColor;
		} else if (CatchManager.g.CanCatch) {
			PercentageFilled = 1f;
			CatchState = CatchState.CanCatch;
		} else {
			Color = _chargingColor;
			PercentageFilled = CatchManager.g.CatchChargePercentage;
			CatchState = CatchState.WaitForCatch;
		}
	}
}
