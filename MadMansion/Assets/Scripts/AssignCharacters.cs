using UnityEngine;
using System.Collections;

public class AssignCharacters : MonoBehaviour {

	[SerializeField]
	private GameObject[] _players;

	void Update () {
		if (PlayerInputManager.g.Ghost != null &&
			PlayerInputManager.g.Hunter != null) {
			for (int i = 0; i < _players.Length; i++) {
				NPCController npcController = _players[i].GetComponent<NPCController>();
				npcController.enabled = true;
				npcController.InitRoomPattern(RoomManager.g.RandomRoomList(3));
			}

			int hunterIndex = Random.Range(0,_players.Length);
			int ghostIndex = Random.Range(0,_players.Length);
			if (ghostIndex == hunterIndex) {
				ghostIndex = (hunterIndex + 1) % _players.Length;
			}
			_players[hunterIndex].GetComponent<HunterController>().enabled = true;
			_players[ghostIndex].GetComponent<GhostController>().enabled = true;

			this.enabled = false;

			HauntManager.g.StartHauntCharge(); // XXX: TODO: Use Message Passing to reduce coupling
			PossessionManager.g.StartPossessionCharge(); // XXX: TODO: Use Message Passing to reduce coupling
		}
	}
}
