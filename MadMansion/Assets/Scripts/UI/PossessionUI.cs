using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum PossessionState {
	WaitForPossession,
	CanPossess,
	MustPossess
}

public class PossessionUI : MonoBehaviour {

	[SerializeField]
	private Sprite _waitForPossessionImage;
	[SerializeField]
	private Sprite _canPossessImage;
	[SerializeField]
	private Sprite _mustPossessImage;
	[SerializeField]
	private Image _radialTimerRenderer;

	[SerializeField]
	private Color _chargingColor;
	[SerializeField]
	private Color _mustPossessColor;
	[SerializeField]
	private AnimationCurve _forcePossessionPulseCurve;
	[SerializeField]
	private AnimationCurve _forcePossessionPulseSpeedCurve;

	private RectTransform _rectTransform;

	private PossessionState _possessionState;
	public PossessionState PossessionState {
		set {
			_possessionState = value;
			switch (_possessionState) {
				case PossessionState.CanPossess:
					_spriteRenderer.sprite = _canPossessImage;
					break;
				case PossessionState.MustPossess:
					_spriteRenderer.sprite = _mustPossessImage;
					break;
				default:
					_spriteRenderer.sprite = _waitForPossessionImage;
					break;
			}
		}
	}

	private Image _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<Image>();
		_radialTimerRenderer.material = new Material(_radialTimerRenderer.material);
		_rectTransform = GetComponent<RectTransform>();
	}

	void Start () {
		PossessionState = PossessionState.WaitForPossession;
		_radialTimerRenderer.material.SetFloat ("_Fraction", 1f);
	}

	void Update () {
		UpdatePossessionTimer();
	}

	public float PercentageFilled {
		set { _radialTimerRenderer.material.SetFloat ("_Fraction", value); }
	}

	public Color Color {
		set { _radialTimerRenderer.material.SetColor ("_Color", value); }
	}

	private float _counter = 0f;
	private void UpdatePossessionTimer () {
		if (PossessionManager.g.MustPossess) {
			_counter += Time.deltaTime;
			PossessionState = PossessionState.MustPossess;
			_rectTransform.localScale = Vector2.one * (1 + Mathf.PingPong(Mathf.Pow(_counter, 1.1f), 1f) * _forcePossessionPulseCurve.Evaluate(PossessionManager.g.PossessionForcedPercentage));
		} else if (PossessionManager.g.CanPossess) {
			_counter += Time.deltaTime;
			PercentageFilled = PossessionManager.g.PossessionForcedPercentage;
			PossessionState = PossessionState.CanPossess;
			Color = _mustPossessColor;
			_rectTransform.localScale = Vector2.one * (1 + Mathf.PingPong(Mathf.Pow(_counter, 1.1f), 1f) * _forcePossessionPulseCurve.Evaluate(PossessionManager.g.PossessionForcedPercentage));
		} else {
			_counter = 0f;
			Color = _chargingColor;
			PercentageFilled = PossessionManager.g.PossessionChargePercentage;
			PossessionState = PossessionState.WaitForPossession;
			_rectTransform.localScale = Vector2.one;
		}
	}
}
