using UnityEngine;
using System.Collections;

public enum CatchState {
	WaitForPossession,
	CanPossess,
	MustPossess
}

public class CatchUI : MonoBehaviour {

	[SerializeField]
	private Sprite _waitForPossessionImage;
	[SerializeField]
	private Sprite _canPossessImage;
	[SerializeField]
	private Sprite _mustPossessImage;
	[SerializeField]
	private Renderer _radialTimerRenderer;

	[SerializeField]
	private Color _chargingColor;
	[SerializeField]
	private Color _mustPossessColor;

	private CatchState _possessionState;
	public CatchState CatchState {
		set {
			_possessionState = value;
			switch (_possessionState) {
				case CatchState.CanPossess:
					_spriteRenderer.sprite = _canPossessImage;
					break;
				case CatchState.MustPossess:
					_spriteRenderer.sprite = _mustPossessImage;
					break;
				default:
					_spriteRenderer.sprite = _waitForPossessionImage;
					break;
			}
		}
	}

	private SpriteRenderer _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		CatchState = CatchState.WaitForPossession;
		_radialTimerRenderer.material.SetFloat ("_Cutoff", 0f);
	}

	void Update () {
		UpdatePossessionTimer();
	}

	public float PercentageFilled {
		set { _radialTimerRenderer.material.SetFloat ("_Cutoff", 1f-value); }
	}

	public Color Color {
		set { _radialTimerRenderer.material.SetColor ("_Color", value); }
	}

	private void UpdatePossessionTimer () {
		if (PossessionManager.g.MustPossess) {
			CatchState = CatchState.MustPossess;
		} else if (PossessionManager.g.CanPossess) {
			PercentageFilled = PossessionManager.g.PossessionForcedPercentage;
			CatchState = CatchState.CanPossess;
			Color = _mustPossessColor;
		} else {
			Color = _chargingColor;
			PercentageFilled = PossessionManager.g.PossessionChargePercentage;
			CatchState = CatchState.WaitForPossession;
		}
	}
}
