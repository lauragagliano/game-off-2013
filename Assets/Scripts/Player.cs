using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public int money;
	public int curHealth;
	const int BASE_HEALTH = 1;
	public int curShields;

	public bool IsDead { get; private set; }

	
	// Abilities
	public BluePower bluePower;
	bool isUsingSlowdown;
	float slowDownStrength = 20f;
	// TODO If we add improved slowdown upgrade
	//const int UPGRADED_SLOWDOWN_STRENGTH = 25f;

	public RedPower redPower;
	public GreenPower greenPower;
	int shieldStrength = 3;
	// TODO If we add shield strength upgrade, do it in this class
	//const int UPGRADED_SHIELD_STRENGTH = 2;
	
	public RGB playerRGB;
	public float MAGNET_DIST = 10.0f;
	Inventory inventory;

	public int WildcardCount { get; private set; }
	
	public List<GameObject> pickups;
	const int POWER_UNIT = 1;
	public AudioClip pickupSound;
	public AudioClip deathSound;
	public AudioClip slowDownSound;
	public AudioClip speedUpSound;
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
	public Vector3 perceivedVelocity;
	
	
	#region #1 Awake and Update
	void Awake ()
	{
		LinkSceneReferences ();
		LinkNodeReferences ();
		LinkComponents ();
		
		// Set our health and powers
		curHealth = BASE_HEALTH;

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
	void LinkSceneReferences ()
	{
		GameObject treadmillGO = GameObject.FindGameObjectWithTag (Tags.TREADMILL);
		treadmill = treadmillGO.GetComponent<Treadmill> ();
	}
	
	void Update ()
	{
		MatchSpeedToTreadmill ();

		TryMove ();
		TryActivateAbilities ();

		CheckShieldTimeout ();
		CheckSlowDownTimeout ();
		
		ClampToWorldYZ (worldYClamp, worldZClamp);
		// If no colors are active, go neutral
		if (!bluePower.IsPowerActive () && !redPower.IsPowerActive () && !greenPower.IsPowerActive ()) {
			ChangeColors (ColorWheel.neutral);
		}
		RenderCurrentColor ();
		PullNearbyPickups ();
	}
	
	/*
	 * Adjusts the player's movespeed left and right to keep up with the treadmill. This allows us to
	 * make challenges that stay consistently possible for the player as the board speeds up.
	 */
	void MatchSpeedToTreadmill ()
	{
		movespeed = treadmill.scrollspeed;
		
		// Set animation playback speed on animation to match new movespeed
		float ANIM_NORMAL_RUNSPEED = 30.0f;
		playerGeo.animation ["pigment_run"].speed = movespeed / ANIM_NORMAL_RUNSPEED;
	}
	
	/*
	 * Refresh the current color on the character
	 */
	void RenderCurrentColor ()
	{
		// TODO Why on Update???
		PigmentBody body = (PigmentBody)playerGeo.GetComponent<PigmentBody> ();
		body.SetColor (playerRGB.color);
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
	 * Logic for using abilities. First try to activate the power that
	 * the player has pushed the key for. Then try and use the ability
	 * if it is off of cooldown.
	 */
	void TryActivateAbilities ()
	{
		if (Input.GetKeyDown ("j")) {
			if (redPower.IsChargedAndReady ()) {
				SetActivePower (redPower, greenPower, bluePower);
				ChangeColors (ColorWheel.red);
			}
			if (redPower.IsPowerActive () && !redPower.AbilityOnCooldown ()) {
				Laser ();
			}
		} else if (Input.GetKeyDown ("k")) {
			if (greenPower.IsChargedAndReady ()) {
				SetActivePower (greenPower, redPower, bluePower);
				ChangeColors (ColorWheel.green);
				RaiseShield ();
			}
		} else if (Input.GetKeyDown ("l")) {
			if (bluePower.IsChargedAndReady ()) {
				SetActivePower (bluePower, redPower, greenPower);
				SlowDown ();
				ChangeColors (ColorWheel.blue);
			}
		}
	}
	
	/*
	 * Helper method to set the active power and turn off others if in use.
	 */
	void SetActivePower (Power activePower, Power inactivePower1, Power inactivePower2)
	{
		activePower.ActivatePower ();
		// Add this back if we want powers to stomp eachother
//		DeactivatePowerIfActive (inactivePower1);
//		DeactivatePowerIfActive (inactivePower2);
	}
	
	/*
	 * Helper method to deactivate power only if it's in use.
	 */
	void DeactivatePowerIfActive (Power power)
	{
		if (power.IsPowerActive ()) {
			power.ResetPower ();
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
		float noMagnetDist = transform.collider.bounds.extents.x;
		float pullDistance;
		foreach (GameObject pickup in pickups) {
			pullDistance = noMagnetDist;
			Pickup pickupScript = pickup.GetComponent<Pickup> ();
			if (pickup.GetComponent<RGB> () != null) {
				if (HasMagnetForColor (pickup.GetComponent<RGB> ().color)) {
					pullDistance = MAGNET_DIST;
				}
			}
			if (Vector3.SqrMagnitude (pickup.transform.position - transform.position) <= (Mathf.Pow (pullDistance, 2) + 
				pickupScript.size)) {
				pickupScript.StartCollecting (gameObject);
			}
		}
	}
	
	/*
	 * When a pickup is gathered, increment stats for the player and game. Play
	 * the sounds needed and charge the player's meters.
	 */
	public void CollectPickup (GameObject pickup)
	{
		if (pickup.GetComponent<CrystalPickup> () != null) {
			RGB pickupRGB = pickup.GetComponent<RGB> ();
			audio.PlayOneShot (pickupSound);
			Power powerToCharge = GetPowerForColor (pickupRGB);
			if (powerToCharge.IsChargedAndReady ()) {
				// Logic for spillover goes here
			} else {
				powerToCharge.AddPower (POWER_UNIT);
			}
			GameManager.Instance.AddPoint ();
			// Add up our money
			AddMoney (1);
			
			ForgetPickup (pickup);
		} else if (pickup.CompareTag (Tags.WILDCARD)) {
			AwardWildcard ();
		} else {
			Debug.Log ("Player encountered unknown Pickup! Handle this with a new tag on the pickup.");
		}
	}
	
	/*
	 * When a player collides with a black block, take damage.
	 */
	public void CollideWithBlock ()
	{
		if (curShields > 0) {
			TakeShieldHit (1);
		} else {
			// Subtract the health
			curHealth = curHealth - 1;
		}
		
		// Handle death
		if (curHealth <= 0) {
			Die ();
		}
	}
	
	/*
	 * Increments the number of wildcards the player has collected.
	 */
	public void AwardWildcard ()
	{
		WildcardCount++;
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
	
	public void Die ()
	{
		IsDead = true;
		
		GameManager.Instance.EndRun ();
		gameObject.SetActive (false);
		playerGeo.animation.Stop ();
		
		PigmentBody body = (PigmentBody)playerGeo.GetComponent<PigmentBody> ();
		body.ReplaceWithRagdoll ();
	}
	
	/*
	 * Spawns the player at the specified position
	 */
	public void Spawn (Vector3 spawnPosition)
	{
		if (!IsDead) {
			InitializeStatsOnSpawn ();
		} else {
			InitializeStatsOnRevive ();
			
			// Restore ragdolled limbs
			PigmentBody body = (PigmentBody)playerGeo.GetComponent<PigmentBody> ();
			body.RestoreFromRagdoll ();
		}
		
		// Snap to spawn position
		transform.position = spawnPosition;
		
		IsDead = false;
	}
	
	/*
	 * Called when the ragdoll is done bringing the body parts back together.
	 */
	public void OnRagdollRestored()
	{
		gameObject.SetActive (true);
		
		float explosionRadius = 25;
		GameObject[] blackblocks = GameObject.FindGameObjectsWithTag(Tags.BLOCK);
		foreach(GameObject block in blackblocks)
		{
			if(Vector3.SqrMagnitude(block.transform.position - transform.position) <= Mathf.Pow(explosionRadius, 2.0f))
			{
				block.GetComponent<BlockLogic>().BlowUp (transform.position, 200, explosionRadius);
			}
		}
		
		GameManager.Instance.OnReviveDone();
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
	
	#region #4 Player Powers and Abilities

	void ChangeColors (ColorWheel color)
	{
		//TODO Refactor this dumb switch
		switch (color) {
		case ColorWheel.blue:
			Camera.main.backgroundColor = ColorManager.Instance.blue.color;
			break;
		case ColorWheel.red:
			Camera.main.backgroundColor = ColorManager.Instance.red.color;
			break;
		case ColorWheel.green:
			Camera.main.backgroundColor = ColorManager.Instance.green.color;
			break;
		case ColorWheel.neutral:
			Camera.main.backgroundColor = ColorManager.Instance.black.color;
			break;
		}
		playerRGB.color = color;/*
		foreach (GameObject pickup in pickups) {
			if (pickup.CompareTag (Tags.PICKUP)) {
				RGB pickupRGB = pickup.GetComponent<RGB> ();
				pickupRGB.color = color;
				pickupRGB.Refresh ();
			}
		}*/
	}
	
	/*
	 * Move the character in the specified direction with the specfied speed.
	 */
	void Move (Vector3 direction, float speed)
	{
		Vector3 movement = (direction.normalized * speed);

		// Update perceived velocity vector
		perceivedVelocity = movement + new Vector3 (0.0f, 0.0f, (treadmill.scrollspeed));
		
		// Apply movement vector
		movement *= Time.deltaTime;
		CharacterController biped = GetComponent<CharacterController> ();
		biped.Move (movement);
	}

	/*
	 * Cast our laser ability if cooldown is ready.
	 */
	void Laser ()
	{
		redPower.UseAbility ();
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
	
	/*
	 * Put up shields for the player. Set our shield value, play sound,
	 * and show shield.
	 */
	public void RaiseShield ()
	{
		audio.PlayOneShot (shieldUpSound);
		curShields = shieldStrength;
		shieldObject.SetActive (true);
	}
	
	/*
	 * Turn off shields. Should be called when green power times out or player
	 * is out of shield hits.
	 */
	public void LowerShields ()
	{
		audio.PlayOneShot (shieldDownSound);
		curShields = 0;
		shieldObject.SetActive (false);
		greenPower.ResetPower ();
	}
	
	/*
	 * Calculate a hit to the shields (as opposed to health). Play the right sound
	 * depending on a shield being hit or going down.
	 */
	public void TakeShieldHit (int loss)
	{
		int newShields = curShields - loss;
		if (newShields > 0) {
			audio.PlayOneShot (shieldHitSound);
		} else if (newShields == 0) {
			LowerShields ();
		}
		curShields = newShields;
	}
	
	/*
	 * If shields are up but green power is all out, turn off shields.
	 */
	void CheckShieldTimeout ()
	{
		if (curShields > 0 && !greenPower.IsPowerActive ()) {
			LowerShields ();
		}
	}
	
	void CheckSlowDownTimeout ()
	{
		if (isUsingSlowdown && !bluePower.IsPowerActive ()) {
			GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ().ResumeTreadmill ();
			isUsingSlowdown = false;
			audio.PlayOneShot (speedUpSound);
		}
	}
	
	public void SlowDown ()
	{
		isUsingSlowdown = true;
		//TODO This is a case where we could have a protected get component call that null checks.
		audio.PlayOneShot (slowDownSound);
		GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ().TemporarySlowDown (slowDownStrength);
		bluePower.ActivatePower ();
	}
	#endregion
	
	#region #5 Player Upgrades
	
	/*
	 * Call this to make sure new powers or necessary resets occur.
	 */
	void InitializeStatsOnSpawn ()
	{
		pickups = new List<GameObject> ();
		
		
		// Give player their upgrades
		if (inventory.HasItem (ItemNames.BLUE_METER_UPGRADE)) {
			bluePower.UpgradeMaximumCharge ();
		}
		if (inventory.HasItem (ItemNames.RED_METER_UPGRADE)) {
			redPower.UpgradeMaximumCharge ();
		}
		if (inventory.HasItem (ItemNames.GREEN_METER_UPGRADE)) {
			greenPower.UpgradeMaximumCharge ();
		}
		if (inventory.HasItem (ItemNames.LASER_COOLDOWN_UPGRADE)) {
			redPower.UpgradeCooldown ();
		}
		
		// Also reset the same stats that should be reset on revive
		InitializeStatsOnRevive();
	}
	
	/*
	 * This should be called to reset stats when the player comes back to life or first time spawn.
	 */
	void InitializeStatsOnRevive ()
	{
		redPower.ResetPower ();
		greenPower.ResetPower ();
		greenPower.curValue = 0;
		bluePower.curValue = 0;
		isUsingSlowdown = false;
		curHealth = 1;
		
		// Reset the number of wildcards that have been collected.
		WildcardCount = 0;
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