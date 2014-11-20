using UnityEngine;
using System.Collections;

public class AssignCharacters : MonoBehaviour {

	[SerializeField]
	private GameObject[] _players;

	void Update () {
		if (PlayerInputManager.g.Ghost != null &&
			PlayerInputManager.g.Hunter != null) {
			int hunterIndex = Random.Range(0,_players.Length);
			int ghostIndex = Random.Range(0,_players.Length);
			if (ghostIndex == hunterIndex) {
				ghostIndex = (hunterIndex + 1) % _players.Length;
			}
			_players[hunterIndex].GetComponent<HunterController>().enabled = true;
			_players[ghostIndex].GetComponent<GhostController>().enabled = true;

			this.enabled = false;
		}
	}
}
