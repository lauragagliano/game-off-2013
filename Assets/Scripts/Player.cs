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
	public AudioClip pickupSound;
	public AudioClip damageSound;
	public AudioClip deathSound;
	public AudioClip slowDownSound;
	public AudioClip regenHealthSound;
	
	#region #1 Awake and Update
	void Awake ()
	{
		curHealth = maxHealth;
		playerColor = (ColorLogic)GetComponent<ColorLogic> ();
		bluePower = (BluePower)GetComponent<BluePower> ();
		redPower = (RedPower)GetComponent<RedPower> ();
		greenPower = (GreenPower)GetComponent<GreenPower> ();
	}
	#endregion
	
	#region #2 Player and Block interaction
	public void HandleBlockCollision (ColorLogic blockColor)
	{
		bool goodCollision = playerColor.isCompatible (blockColor);
		if (goodCollision) {
			audio.PlayOneShot (pickupSound);
		} else {
			audio.PlayOneShot (damageSound);
		}
		switch (blockColor.color) {
		case ColorWheel.blue:
			if (!goodCollision) {
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
			} else if (redPower.IsCharged ()) {
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
		if (curHealth > 0 + loss) {
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
	public void Laser ()
	{
		if (redPower.IsCharged ()) {
			redPower.ExhaustPower ();
					
			const float LASER_HALFWIDTH = 2.5f;
			const float LASER_LENGTH = 60.0f;
			
			GameObject[] allBlocks = GameObject.FindGameObjectsWithTag ("Block");
			foreach (GameObject block in allBlocks) {
				Vector3 directionToBlock = block.transform.position - transform.position;
				float distanceToBlockRelativeToMyForward = Vector3.Project (directionToBlock, transform.forward).magnitude;
				
				// Note this will not work if the blocks are rotated,and it assumes they are square
				float blockWidth = block.collider.bounds.extents.x;
				float blockHeight = blockWidth;
				// Is the block in range of my laser?
				if (distanceToBlockRelativeToMyForward <= LASER_LENGTH + blockHeight) {
					// Compare distance along my X axis
					float distanceToBlockRelativeToMyRight = Vector3.Project (directionToBlock, transform.right).magnitude;
					if (distanceToBlockRelativeToMyRight <= (LASER_HALFWIDTH + blockWidth)) {
						ColorLogic colorLogic = block.GetComponent<ColorLogic> ();
						if (colorLogic.color == ColorWheel.black) {
							block.GetComponent<BlockLogic> ().BlowUp ();
						}
					}
				}
			}
		}
	}
	
	public void RegenHealth ()
	{
		if (greenPower.IsCharged ()) {
			audio.PlayOneShot (regenHealthSound);
			curHealth = maxHealth;
			greenPower.ExhaustPower ();
		}
	}
	
	public void SlowDown ()
	{
		if (bluePower.IsCharged ()) {
			//TODO This is a case where we could have a protected get component call that null checks.
			audio.PlayOneShot (slowDownSound);
			GameObject.Find (ObjectNames.GROUND).GetComponent<Treadmill> ().SlowDown ();
			bluePower.ExhaustPower ();
		}
	}
	#endregion
}