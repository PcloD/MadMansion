using UnityEngine;
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
	private Renderer _radialTimerRenderer;

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

	private SpriteRenderer _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		CatchState = CatchState.WaitForCatch;
		_radialTimerRenderer.material.SetFloat ("_Cutoff", 0f);
	}

	void Update () {
		UpdateCatchTimer();
	}

	public float PercentageFilled {
		set { _radialTimerRenderer.material.SetFloat ("_Cutoff", 1f-value); }
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
