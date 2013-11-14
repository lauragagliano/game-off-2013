using UnityEngine;
using System.Collections;

/*
 * "Dumb" class that just holds and manipulates values.
 * Avoid putting logic here as this class is used for all
 * three colors.
 */
public abstract class Power : MonoBehaviour
{
	public AudioClip powerReadySound;
	protected ColorWheel color;
	public int curValue = 0;
	public int MaxValue = 20;
	
	public void AddPower (int amount)
	{
		if (curValue < MaxValue) {
			curValue += amount;
			if (curValue == MaxValue) {
				audio.PlayOneShot (powerReadySound);
			}
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
		curValue = MaxValue;
	}
	
	public bool IsCharged ()
	{
		return curValue == MaxValue;
	}
	
	public void ExhaustCharge ()
	{
		curValue = 0;
	}
}
