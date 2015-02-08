using UnityEngine;
using System.Collections;

public class AssignCharactersOnStart : MonoBehaviour {

	[SerializeField]
	private GameObject[] _players;

	void OnEnable ()
	{
		Events.g.AddListener<StartGameEvent>(Assign);
	}

	void OnDisable ()
	{
		Events.g.RemoveListener<StartGameEvent>(Assign);
	}

	private void Assign (StartGameEvent e)
	{
		// Handle event here
		for (int i = 0; i < _players.Length; i++) {
			NPCController npcController = _players[i].GetComponent<NPCController>();
			GhostController ghostController = _players[i].GetComponent<GhostController>();
			HunterController hunterController = _players[i].GetComponent<HunterController>();

			if (e.gameMode == GameMode.Practice) {
				hunterController.AlwaysRevealed = true;
				ghostController.AlwaysRevealed = true;
			}

			npcController.enabled = true;
			hunterController.enabled = false;
			ghostController.enabled = false;
			int minimumFurnitureCount = Mathf.Max(0, RoomManager.g.RoomCount - 1);
			int maximumFurnitureCount = RoomManager.g.RoomCount;
			npcController.InitFurniturePattern(RoomManager.g.RandomFurnitureList(Random.Range(minimumFurnitureCount,maximumFurnitureCount)));
		}

		int hunterIndex = Random.Range(0,_players.Length);
		int ghostIndex = Random.Range(0,_players.Length);
		if (ghostIndex == hunterIndex) {
			ghostIndex = (hunterIndex + 1) % _players.Length;
		}
		_players[hunterIndex].GetComponent<HunterController>().enabled = true;
		_players[ghostIndex].GetComponent<GhostController>().enabled = true;
	}
}