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
	bool isPowerActive;
	float powerDuration = 5;
	const float UPGRADED_DURATION = 20;
	RBTimer powerTimer = new RBTimer ();

	// Ability behavior
	RBTimer abilityCooldownTimer = new RBTimer ();
	float abilityCooldown = 3;
	const float UPGRADED_COOLDOWN = 1;
		
	/*
	 * Reset all timers. Set current value to default (0). Useful when starting
	 * a game.
	 */
	public void ResetPower ()
	{
		isPowerActive = false;
		abilityCooldownTimer.StopTimer ();
		curValue = 0;
	}
	
	void Update ()
	{
		// Stop our timers when power and abilities are done being used
		if (IsPowerActive ()) {
			curValue = Mathf.Max (curValue - ((maxValue /powerDuration) * Time.deltaTime), 0);
			if (abilityCooldownTimer.IsTimeUp ()) {
				abilityCooldownTimer.StopTimer ();
			}
			if (curValue <= 0) {
				isPowerActive = false;
				abilityCooldownTimer.StopTimer ();
			}
		}
	}
	
	/*
	 * Get how full the power is (used for GUI display).
	 */
	public float GetFillPercentage ()
	{
		return ((float)curValue / maxValue);
	}
	
	/*
	 * Add a provided amount of power.
	 */
	public void AddPower (int amount)
	{
		float newVal = curValue + amount;
		if (newVal < maxValue) {
			curValue = newVal;
		} else if (newVal >= maxValue) {
			curValue = maxValue;
		}
		if (curValue == maxValue && !IsPowerActive ()) {
			audio.PlayOneShot (powerReadySound);
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
		return curValue == maxValue && !IsPowerActive ();
	}
	
	/*
	 * Return whether the power is active. Abilities tied to powers shouldn't
	 * be usable unless the power is active.
	 */
	public bool IsPowerActive ()
	{
		return isPowerActive;
	}
	
	/*
	 * Call this to use all energy and activate the power for its
	 * duration.
	 */
	public void ActivatePower ()
	{
		isPowerActive = true;
	}
	
	/*
	 * Use the ability associated with this power. This sets the cooldown timer
	 * and the ability behavior itself is controlled by the player.
	 */
	public void UseAbility ()
	{
		if (IsPowerActive () && !abilityCooldownTimer.IsRunning ()) {
			abilityCooldownTimer.StartTimer (abilityCooldown);
		}
	}
	
	/*
	 * Return true if the ability is on cooldown (unusable).
	 */
	public bool AbilityOnCooldown ()
	{
		return abilityCooldownTimer.IsRunning ();
	}
	
	/*
	 * Set our maximum charge value to the upgraded value.
	 */
	public void UpgradeMaximumCharge ()
	{
		maxValue = UPGRADED_MAX;
	}
	
	/*
	 * Set our cooldown duration to the upgraded value.
	 */
	public void UpgradeCooldown ()
	{
		abilityCooldown = UPGRADED_COOLDOWN;
	}
}
