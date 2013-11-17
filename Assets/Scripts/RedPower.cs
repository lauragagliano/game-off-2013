using UnityEngine;
using System.Collections;

public class RedPower : Power
{
	//TODO unless some logic is added in these power classes, we can just set
	// player's power colors on player load and eliminate these subclasses.
	void Awake ()
	{
		color = ColorWheel.red;
	}
}
