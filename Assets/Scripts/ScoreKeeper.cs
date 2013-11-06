using UnityEngine;
using System.Collections;

public class ScoreKeeper : Singleton<ScoreKeeper>
{
	public int gatesCrossed = 0;
	
	public void ScorePoint ()
	{
		gatesCrossed++;
	}
}
