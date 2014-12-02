using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayEndGameText : MonoBehaviour {

	[SerializeField]
	private Text _endText;
	[SerializeField]
	private Text _endTextReason;
	[SerializeField]
	private string _hunterWinText;
	[SerializeField]
	private string _ghostWinText;
	[SerializeField]
	private string _noPlayerWinText;
	[SerializeField]
	private string _hunterCaughtGhostText;
	[SerializeField]
	private string _ghostHauntedHouseText;
	[SerializeField]
	private string _hunterCaughtGhostInSameBodyText;
	[SerializeField]
	private string _hunterCaughtInnocentText;

	void OnEnable ()
	{
		Events.g.AddListener<EndGameEvent>(GameEnded);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<EndGameEvent>(GameEnded);
	}

	private void GameEnded (EndGameEvent e) {
		switch (e.winner) {
			case Player.GhostPlayer:
				_endText.text = _ghostWinText;
				break;
			case Player.HunterPlayer:
				_endText.text = _hunterWinText;
				break;
			case Player.NoPlayer:
				_endText.text = _noPlayerWinText;
				break;
		}
		switch (e.rationale) {
			case EndReason.HunterCaughtGhost:
				_endTextReason.text = _hunterCaughtGhostText;
				break;
			case EndReason.GhostHauntedHouse:
				_endTextReason.text = _ghostHauntedHouseText;
				break;
			case EndReason.HunterCaughtGhostInSameBody:
				_endTextReason.text = _hunterCaughtGhostInSameBodyText;
				break;
			case EndReason.HunterCaughtInnocent:
				_endTextReason.text = _hunterCaughtInnocentText;
				break;
		}
	}

}
