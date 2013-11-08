using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public int curHealth;
	int maxHealth = 10;
	public BluePower bluePower;
	public RedPower redPower;
	public Power greenPower;
	public ColorLogic playerColor;
	
	const int POWER_UNIT = 1;

	#region #1 Awake and Update
	void Awake ()
	{
		curHealth = maxHealth;
		playerColor = (ColorLogic) GetComponent<ColorLogic> ();
		bluePower = (BluePower) GetComponent<BluePower> ();
		redPower = (RedPower) GetComponent<RedPower> ();
		greenPower = (GreenPower) GetComponent<GreenPower> ();
	}
	#endregion
	
	#region #2 Player and Block interaction
	public void HandleBlockCollision (ColorLogic blockColor)
	{
		bool goodCollision = playerColor.isCompatible (blockColor);
		switch (blockColor.color) {
		case ColorWheel.blue:
			if (!goodCollision){
				LoseHealth (1);
			} else if (bluePower.IsCharged ()) {
				// Logic for spillover could go here....
			} else {
				bluePower.AddPower (POWER_UNIT);
			}
			break;
		case ColorWheel.red:
			if (!goodCollision) {
				LoseHealth (1);
			}else if (redPower.IsCharged ()) {
				// Logic for spillover could go here....
			} else {
				redPower.AddPower (POWER_UNIT);
			}
			break;
		case ColorWheel.green:
			if (!goodCollision) {
				LoseHealth (1);
			} else if (greenPower.IsCharged ()) {
				// Logic for spillover could go here....
			} else {
				greenPower.AddPower (POWER_UNIT);
			}
			break;
		case ColorWheel.black:
			Die ();
			break;
		}
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
			redPower.ExhaustPower ();
		}
	}

	public void RegenHealth ()
	{
		if (greenPower.IsCharged ()) {
			curHealth = maxHealth;
			greenPower.ExhaustPower ();
		}
	}
	
	public void SlowDown ()
	{
		if (bluePower.IsCharged ()) {
			//TODO This is a case where we could have a protected get component call that null checks.
			GameObject.Find (ObjectNames.GROUND).GetComponent<Treadmill> ().SlowDown ();
			bluePower.ExhaustPower ();
		}
	}
	#endregion
}