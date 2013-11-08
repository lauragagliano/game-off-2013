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
	
	void Awake ()
	{
		curHealth = maxHealth;
		playerColor = (ColorLogic) GetComponent<ColorLogic> ();
		bluePower = (BluePower) GetComponent<BluePower> ();
		redPower = (RedPower) GetComponent<RedPower> ();
		greenPower = (GreenPower) GetComponent<GreenPower> ();
	}
	
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
}