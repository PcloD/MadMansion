using UnityEngine;
using System.Collections;

public enum HauntState {
	WaitForHaunt,
	CanHaunt,
	Haunted
}

[RequireComponent (typeof(SpriteRenderer))]
public class HauntIcon : MonoBehaviour {

	[SerializeField]
	private Sprite _waitForHauntImage;
	[SerializeField]
	private Sprite _hauntedImage;
	[SerializeField]
	private Sprite _canHauntImage;
	[SerializeField]
	private Renderer _radialTimerRenderer;

	private HauntState _hauntState;
	public HauntState HauntState {
		set {
			_hauntState = value;
			switch (_hauntState) {
				case HauntState.CanHaunt:
					_spriteRenderer.sprite = _canHauntImage;
					PercentageFilled = 1f;
					break;
				case HauntState.Haunted:
					_spriteRenderer.sprite = _hauntedImage;
					PercentageFilled = 0f;
					break;
				default:
					_spriteRenderer.sprite = _waitForHauntImage;
					break;
			}
		}
	}

	public float PercentageFilled {
		set { _radialTimerRenderer.material.SetFloat ("_Cutoff", 1f-value); }
	}

	private SpriteRenderer _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		HauntState = HauntState.WaitForHaunt;
		_radialTimerRenderer.material.SetFloat ("_Cutoff", 1f);
	}


}
