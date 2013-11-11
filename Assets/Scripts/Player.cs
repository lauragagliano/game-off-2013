using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public int curHealth;
	int maxHealth = 10;
	public BluePower bluePower;
	public RedPower redPower;
	public GreenPower greenPower;
	public RGB playerRGB;
	
	const int POWER_UNIT = 1;
	
	public AudioClip pickupSound;
	public AudioClip damageSound;
	public AudioClip deathSound;
	public AudioClip slowDownSound;
	public AudioClip regenHealthSound;
	
	#region #1 Awake and Update
	void Awake ()
	{
		curHealth = maxHealth;
		playerRGB = (RGB) GetComponent<RGB> ();
		bluePower = (BluePower) GetComponent<BluePower> ();
		redPower = (RedPower) GetComponent<RedPower> ();
		greenPower = (GreenPower) GetComponent<GreenPower> ();
	}
	#endregion
	
	#region #2 Player and Block interaction
	public void HandleBlockCollision (RGB blockRGB)
	{
		bool goodCollision = playerRGB.isCompatible (blockRGB);
		if (goodCollision) {
			audio.PlayOneShot (pickupSound);
		} else {
			audio.PlayOneShot (damageSound);
			LoseHealth (1);
		}
		if (blockRGB.color == ColorWheel.black) {
			Die ();
			return;
		}
		Power powerToCharge = GetPowerForColor(blockRGB);
		if(powerToCharge.IsCharged()){
			// Logic for spillover goes here
		} else {
			powerToCharge.AddPower (POWER_UNIT);
		}
	}
	
	/*
	 * Map our player's power bars to the color passed in by returning
	 * the power associated with the provided color.
	 */
	Power GetPowerForColor(RGB rgb)
	{
		ColorWheel color = rgb.color;
		Power returnPower = null;
		switch (color) {
		case ColorWheel.blue:
			returnPower = bluePower;
			break;
		case ColorWheel.red:
			returnPower = redPower;
			break;
		case ColorWheel.green:
			returnPower = greenPower;
			break;
		}
		return returnPower;
	}
	
	public void LoseHealth (int loss)
	{
		if (curHealth > 0+loss) {
			curHealth -= loss;
		} else {
			curHealth = 0;
			Die ();
		}
	}
	
	public void Die ()
	{
		gameObject.SetActive (false);
	}
	#endregion
	
	#region #2 Player Powers
	public void Magnet ()
	{
		if (redPower.IsCharged ()) {
			redPower.ExhaustCharge ();
		}
	}
	
	public void RegenHealth ()
	{
		if (greenPower.IsCharged ()) {
			audio.PlayOneShot (regenHealthSound);
			curHealth = maxHealth;
			greenPower.ExhaustCharge ();
		}
	}
	
	public void SlowDown ()
	{
		if (bluePower.IsCharged ()) {
			//TODO This is a case where we could have a protected get component call that null checks.
			audio.PlayOneShot (slowDownSound);
			GameObject.Find (ObjectNames.GROUND).GetComponent<Treadmill> ().SlowDown ();
			bluePower.ExhaustCharge ();
		}
	}
	#endregion
}