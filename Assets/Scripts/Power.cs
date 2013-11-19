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
	
	// Power behavior
	public float curValue = 0;
	float maxValue = 20;
	const float UPGRADED_MAX = 15;
	float duration = 5;
	const float UPGRADED_DURATION = 20;
	
	RBTimer powerTimer = new RBTimer();
	bool isReady = true;
	
	void Update ()
	{
		// Unset our timer when power's done being used
		if (powerTimer.IsRunning ()) {
			if (powerTimer.IsTimeUp ()) {
				isReady = true;
				powerTimer.StopTimer ();
			}
		}
	}
	
	/*
	 * Get how full the power is (used for GUI display).
	 */
	public float GetFillPercentage ()
	{
		return ((float) curValue / maxValue);
	}
	
	/*
	 * Add a provided amount of power.
	 */
	public void AddPower (int amount)
	{
		if (curValue < maxValue) {
			curValue += amount;
			if (curValue == maxValue) {
				audio.PlayOneShot (powerReadySound);
			}
		}
	}
	
	/*
	 * Remove a provided amount of power.
	 */
	public void RemovePower (int amount)
	{
		if (curValue > 0) {
			curValue -= amount;
		}
	}
	
	/*
	 * Fully charge power.
	 */
	public void Charge ()
	{
		curValue = maxValue;
	}
	
	/*
	 * Check if a power is ready to use (off cooldown and enough energy).
	 */
	public bool IsChargedAndReady ()
	{
		return curValue == maxValue && isReady;
	}
	
	/*
	 * Call this to use all energy and activate the power for its
	 * duration.
	 */
	public void UsePower ()
	{
		powerTimer.StartTimer (duration);
		isReady = false;
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
