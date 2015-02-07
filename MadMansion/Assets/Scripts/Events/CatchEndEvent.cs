using UnityEngine;
using System.Collections;

public class CatchEndEvent : GameEvent
{
	public bool catchRight;
	public CatchEndEvent (bool catchRight)
	{
		this.catchRight = catchRight;
	}
}
