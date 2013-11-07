using UnityEngine;
using System.Collections;

public class ScoreKeeper : Singleton<ScoreKeeper>
{
	public int blueScore = 0;
	public int redScore = 0;
	public int yellowScore = 0;
	
	public void ScorePoint (ColorWheel color)
	{
		switch (color) {
		case ColorWheel.red:
			redScore++;
			break;
		case ColorWheel.blue:
			blueScore++;
			break;
		case ColorWheel.yellow:
			yellowScore++;
			break;
		}
	}
}
