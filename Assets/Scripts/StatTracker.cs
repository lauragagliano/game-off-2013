using UnityEngine;
using System.Collections;

public class StatTracker : Singleton<StatTracker>
{
	public int curBluePoints = 0;
	public int curRedPoints = 0;
	public int curYellowPoints = 0;
	int maxPoints = 5;
	
	public void ScorePoint (ColorWheel color)
	{
		switch (color) {
		case ColorWheel.red:
			AddPointToColor (curRedPoints);
			break;
		case ColorWheel.blue:
			AddPointToColor (curBluePoints);
			break;
		case ColorWheel.yellow:
			AddPointToColor (curYellowPoints);
			break;
		}
	}
	
	void AddPointToColor (int pointCounter)
	{
		if (pointCounter < maxPoints) {
			pointCounter++;
		}// This is where we can add 'spillover' ability.
	}
}
