using UnityEngine;
using System.Collections;
using InControl;

public class ControllerAssignmentEvent : GameEvent {
	public InputDevice device;
	public Player player;
	public ControllerAssignmentEvent(InputDevice device, Player player) {
		this.device = device;
		this.player = player;
	}
}
