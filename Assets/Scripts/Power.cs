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
	int maxValue = 20;
	const int UPGRADED_MAX = 15;

	public float GetFillPercentage ()
	{
		return ((float) curValue / maxValue);
	}
	
	public void AddPower (int amount)
	{
		if (curValue < maxValue) {
			curValue += amount;
			if (curValue == maxValue) {
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
		curValue = maxValue;
	}
	
	public bool IsCharged ()
	{
		return curValue == maxValue;
	}
	
	public void ExhaustCharge ()
	{
		curValue = 0;
	}
	
	/*
	 * Set our maximum charge value to the upgraded value.
	 */
	public void UpgradeMaximumCharge ()
	{
		maxValue = UPGRADED_MAX;
	}
}
