using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public int money;
	public int curHealth;
	int maxHealth = 10;
	public BluePower bluePower;
	public RedPower redPower;
	public GreenPower greenPower;
	public RGB playerRGB;
	public float MAGNET_DIST = 10.0f;
	Inventory inventory;
	
	public List<GameObject> pickups;
	const int POWER_UNIT = 1;
	public AudioClip pickupSound;
	public AudioClip deathSound;
	public AudioClip slowDownSound;
	public AudioClip shieldUpSound;
	public AudioClip shieldHitSound;
	public AudioClip shieldDownSound;
	public AudioClip laserSound;
	
	Treadmill treadmill;
	
	GameObject playerGeo;
	GameObject nodeLaser;
	GameObject shieldObject;
	
	public GameObject laserBeamFX;
	float worldZClamp;
	float worldYClamp;
	public float movespeed;
	
	
	#region #1 Awake and Update
	void Awake ()
	{
		LinkSceneReferences();
		LinkNodeReferences ();
		LinkComponents ();
		
		// Set our health and powers
		curHealth = 1;

		// Remember their initial Y and Z position and keep them there forever.
		worldZClamp = transform.position.z;
		worldYClamp = transform.position.y;
		
		// Render initial color
		RenderCurrentColor ();
		
		// Disable shield FX
		shieldObject.SetActive (false);
	}
	
	/*
	 * Sets references to our "node" empty game objects which are used for position and rotation values of the player.
	 */
	void LinkNodeReferences ()
	{
		playerGeo = transform.FindChild ("PlayerGeo").gameObject;
		
		nodeLaser = playerGeo.transform.FindChild ("node_laser").gameObject;
		shieldObject = transform.FindChild ("FX_Shield").gameObject;
	}
	
	/*
	 * Sets references to components on this Player game object or one of its children
	 */
	void LinkComponents ()
	{
		bluePower = (BluePower)GetComponent<BluePower> ();
		redPower = (RedPower)GetComponent<RedPower> ();
		greenPower = (GreenPower)GetComponent<GreenPower> ();
		playerRGB = (RGB)playerGeo.GetComponent<RGB> ();
		inventory = (Inventory)GetComponent<Inventory> ();
	}
	
	/*
	 * Finds and sets references to objects in the scene.
	 */
	void LinkSceneReferences()
	{
		GameObject treadmillGO = GameObject.FindGameObjectWithTag(Tags.TREADMILL);
		treadmill = treadmillGO.GetComponent<Treadmill> ();
	}
	
	void Update ()
	{
		MatchSpeedToTreadmill();
		
		TryMove ();
		TrySwapColor ();
		
		ClampToWorldYZ (worldYClamp, worldZClamp);
		RenderCurrentColor();
		PullNearbyPickups ();
	}
	
	/*
	 * Adjusts the player's movespeed left and right to keep up with the treadmill. This allows us to
	 * make challenges that stay consistently possible for the player as the board speeds up.
	 */
	void MatchSpeedToTreadmill()
	{
		movespeed = treadmill.scrollspeed;
		
		// Set animation playback speed on animation to match new movespeed
		float ANIM_NORMAL_RUNSPEED = 30.0f;
		playerGeo.animation["pigment_run"].speed = movespeed / ANIM_NORMAL_RUNSPEED;
	}
	
	/*
	 * Refresh the current color on the character
	 */
	void RenderCurrentColor ()
	{
		MaterialSet matSet = (MaterialSet) playerGeo.GetComponent<MaterialSet> ();
		matSet.SetColor(playerRGB.color);
	}
	
	/*
	 * Sets the character's position to the specified world Y and worldZ, preventing
	 * him from shifting.
	 */
	void ClampToWorldYZ (float worldY, float worldZ)
	{
		transform.position = new Vector3 (transform.position.x, worldY, worldZ);
	}
	#endregion
	
	#region #2 Input Tries
	/*
	 * Polls input and moves the character accordingly
	 */
	void TryMove ()
	{
		float direction = Input.GetAxis ("Horizontal");
		Move (new Vector3 (direction, 0.0f, 0.0f), movespeed);
		// Translate in place helps hit trigger colliders
		if (direction == 0) {
			transform.Translate (0, 0, 0);
		}
	}
	
	/*
	 * Logic for switching colors or using abilities. If player is
	 * already the color of the button they are pushing, try to use
	 * that color's special power.
	 */
	void TrySwapColor ()
	{
		if (Input.GetKeyDown ("j")) {
			if (playerRGB.color == ColorWheel.red) {
				Laser ();
			} else {
				ChangeColors (ColorWheel.red);
			}
		} else if (Input.GetKeyDown ("k")) {
			if (playerRGB.color == ColorWheel.green) {
				RaiseShield ();
			} else {
				ChangeColors (ColorWheel.green);
			}
		} else if (Input.GetKeyDown ("l")) {
			if (playerRGB.color == ColorWheel.blue) {
				SlowDown ();
			} else {
				ChangeColors (ColorWheel.blue);
			}
		}
	}
	#endregion

	#region #3 Player and Block interaction
	/*
	 * Store a reference to all pickups the player could encounter.
	 */
	public void RememberPickup (GameObject pickup)
	{
		pickups.Add (pickup);
	}
	
	/*
	 * Remove the reference to a pickup in play.
	 */
	public void ForgetPickup (GameObject pickup)
	{
		pickups.Remove (pickup);
	}
	
	/*
	 * Pull in the pickups that are near the player. Adjust the distance if
	 * the player has a magnet.
	 */
	public void PullNearbyPickups ()
	{
		float pullDistance = transform.lossyScale.x * 2;
		foreach (GameObject pickup in pickups) {
			if (HasMagnetForColor (pickup.GetComponent<RGB> ().color)) {
				pullDistance = MAGNET_DIST;
			}
			if (Vector3.SqrMagnitude (pickup.transform.position - transform.position) <= Mathf.Pow(pullDistance, 2)) {
				pickup.GetComponent<BlockLogic> ().SuckUpBlock (gameObject);
			}
		}
	}
	
	/*
	 * When a pickup is gathered, increment stats for the player and game. Play
	 * the sounds needed and charge the player's meters.
	 */
	public void CollectPickup (GameObject pickup)
	{
		RGB pickupRGB = pickup.GetComponent<RGB> ();
		audio.PlayOneShot (pickupSound);
		Power powerToCharge = GetPowerForColor (pickupRGB);
		if (powerToCharge.IsCharged ()) {
			// Logic for spillover goes here
		} else {
			powerToCharge.AddPower (POWER_UNIT);
		}
		GameManager.Instance.AddPoint (pickupRGB.color);
		// Add up our money
		AddMoney (1);
		
		ForgetPickup (pickup);
	}
	
	/*
	 * When a player collides with a black block, take damage.
	 */
	public void CollideWithBlock ()
	{
		LoseHealth (1);
	}
	
	/*
	 * Map our player's power bars to the color passed in by returning
	 * the power associated with the provided color.
	 */
	Power GetPowerForColor (RGB rgb)
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
		int newHealth = curHealth - loss;
		
		// Handle shield hits
		if (newHealth >= 1) {
			if (newHealth > 1) {
				audio.PlayOneShot (shieldHitSound);
			} else if (newHealth == 1) {
				audio.PlayOneShot (shieldDownSound);
				shieldObject.SetActive (false);
			}
		}
		
		// Subtract the health
		curHealth = newHealth;
		
		// Handle death
		if (curHealth <= 0) {
			Die ();
		}
	}
	
	public void Die ()
	{
		gameObject.SetActive (false);
	}
	
		
	/*
	 * Add specified amount of money to the player currency.
	 */
	public void AddMoney (int amount)
	{
		money += amount;
	}
	
	/*
	 * Remove specified amount of money from player. Log warning
	 * if not enough money.
	 */
	public void RemoveMoney (int amount)
	{
		if (money - amount < 0) {
			Debug.LogWarning (string.Format ("Tried to remove more " +
				"money ({0}) than player owns ({1}).", amount, money));
		}
		money -= amount;
	}
	
	#endregion
	
	#region #4 Player Powers
	
	void ChangeColors (ColorWheel color)
	{
		playerRGB.color = color;
		foreach (GameObject pickup in pickups) {
			RGB pickupRGB = pickup.GetComponent<RGB> ();
			pickupRGB.color = color;
			pickupRGB.Refresh ();
		}
	}
	
	/*
	 * Move the character in the specified direction with the specfied speed.
	 */
	void Move (Vector3 direction, float speed)
	{
		Vector3 movement = (direction.normalized * speed);
		movement *= Time.deltaTime;

		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		biped.Move (movement);
	}

	public void Laser ()
	{
		if (redPower.IsCharged ()) {
			redPower.ExhaustCharge ();
			
			// Spawn the laser FX
			GameObject fx = (GameObject)Instantiate (laserBeamFX, nodeLaser.transform.position,
				nodeLaser.transform.rotation);
			fx.transform.parent = nodeLaser.transform;
			Destroy (fx, 1.0f);
			
			audio.PlayOneShot (laserSound);
			
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
						RGB blockRGB = block.GetComponent<RGB> ();
						if (blockRGB.color == ColorWheel.black) {
							Vector3 explosionPosition = transform.position +
								transform.TransformDirection (Vector3.forward * distanceToBlockRelativeToMyForward);
							block.GetComponent<BlockLogic> ().BlowUp (explosionPosition);
						}
					}
				}
			}
		}
	}
	
	public void RaiseShield ()
	{
		if (greenPower.IsCharged ()) {
			greenPower.ExhaustCharge ();
			audio.PlayOneShot (shieldUpSound);
			curHealth = Mathf.Min (curHealth + 1, maxHealth);
			shieldObject.SetActive (true);
		}
	}
	
	public void SlowDown ()
	{
		if (bluePower.IsCharged ()) {
			//TODO This is a case where we could have a protected get component call that null checks.
			audio.PlayOneShot (slowDownSound);
			GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ().SlowDown ();
			bluePower.ExhaustCharge ();
		}
	}
	#endregion
	
	#region #5 Player Upgrades
	
	/*
	 * Call this to make sure new powers or necessary resets occur.
	 */
	public void InitializeStats ()
	{
		pickups = new List<GameObject> ();
		redPower.curValue = 0;
		greenPower.curValue = 0;
		bluePower.curValue = 0;
		curHealth = 1;
		
		if (inventory.HasItem (ItemNames.BLUE_METER_UPGRADE)) {
			bluePower.UpgradeMaximumCharge ();
		}
		if (inventory.HasItem (ItemNames.RED_METER_UPGRADE)) {
			redPower.UpgradeMaximumCharge ();
		}
		if (inventory.HasItem (ItemNames.GREEN_METER_UPGRADE)) {
			greenPower.UpgradeMaximumCharge ();
		}
	}
	
	/*
	 * Return if the player has the magnet powerup for the
	 * provided color.
	 */
	public bool HasMagnetForColor (ColorWheel color)
	{
		switch (color) {
		case ColorWheel.red:
			if (inventory.HasItem (ItemNames.RED_MAGNET)) {
				return true;
			}
			break;
		case ColorWheel.green:
			if (inventory.HasItem (ItemNames.GREEN_MAGNET)) {
				return true;
			}
			break;
		case ColorWheel.blue:
			if (inventory.HasItem (ItemNames.BLUE_MAGNET)) {
				return true;
			}
			break;
		}
		return false;
	}
	#endregion
}