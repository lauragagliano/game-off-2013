using UnityEngine;
using System.Collections;

/*
 * "Dumb" class that just holds and manipulates values.
 * Avoid putting logic here as this class is used for all
 * three colors.
 */
public abstract class Power : MonoBehaviour
{
	protected ColorWheel color;
	public int curValue;
	int maxValue = 5;
	
	public void AddPower (int amount)
	{
		if (curValue < maxValue) {
			curValue += amount;
		}
	}
	
	public void RemovePower (int amount)
	{
		if (curValue > 0) {
			curValue -= amount;
		}
	}
	
	public void Charge ()
	{
		curValue = maxValue;
	}
	
	public bool IsCharged () 
	{
		return curValue == maxValue;
	}
	
	public void ExhaustPower ()
	{
		curValue = 0;
	}
}
