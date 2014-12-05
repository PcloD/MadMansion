using UnityEngine;
using System.Collections;

public class FinishCatchEvent : GameEvent {
	public CharacterMotor hunter;
	public CharacterMotor guess;
	public FinishCatchEvent (CharacterMotor hunter, CharacterMotor guess) {
		this.hunter = hunter;
		this.guess = guess;
	}
}
