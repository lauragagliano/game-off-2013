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
	public GameObject[] pickups;

	const int POWER_UNIT = 1;
	public AudioClip pickupSound;
	public AudioClip deathSound;
	public AudioClip slowDownSound;
	public AudioClip shieldUpSound;
	public AudioClip shieldHitSound;
	public AudioClip shieldDownSound;
	public AudioClip laserSound;
	
	GameObject playerGeo;
	GameObject nodeLaser;
	public GameObject laserBeamFX;
	
	float worldZClamp;
	public float movespeed = 20.0f;
	
	
	#region #1 Awake and Update
	void Awake ()
	{
		LinkNodeReferences();
		
		// Set our health and powers
		curHealth = 1;
		playerRGB = (RGB) GetComponent<RGB> ();
		bluePower = (BluePower) GetComponent<BluePower> ();
		redPower = (RedPower) GetComponent<RedPower> ();
		greenPower = (GreenPower) GetComponent<GreenPower> ();
		
		// Remember their initial Z position and keep them there forever.
		worldZClamp = transform.position.z;
		// Initialize the colors according to the level rules
		playerRGB = (RGB)playerGeo.GetComponent<RGB> ();
		// Cycle and render colors
		RenderCurrentColor ();
	}
	
	/*
	 * Sets references to our "node" empty game objects which are used for position and rotation values of the player.
	 */
	void LinkNodeReferences()
	{
		playerGeo = transform.FindChild("PlayerGeo").gameObject;
		nodeLaser = playerGeo.transform.FindChild("node_laser").gameObject;
	}
	
	void Update ()
	{
		TryMove ();
		TrySwapColor ();
		ClampToWorldZ (worldZClamp);
		playerRGB.Refresh ();
	}
	
	/*
	 * Refresh the current color on the character
	 */
	void RenderCurrentColor ()
	{
		playerRGB.Refresh ();
	}
	
	/*
	 * Sets the character's position to the specified worldZ, preventing
	 * him from shifting.
	 */
	void ClampToWorldZ (float worldZ)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, worldZ);
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
	 * When a player collides with a block, perform the required action based
	 * on the RGB of the block. If the player is the same color as the block,
	 * suck it up. If it's not, take damage. If the block is black, hit
	 * the player's shields.
	 */
	public void HandleBlockCollision (RGB blockRGB)
	{
		bool goodCollision = playerRGB.isCompatible (blockRGB);
		if (goodCollision) {
			audio.PlayOneShot (pickupSound);
		} 
		if (blockRGB.color == ColorWheel.black) {
			LoseHealth (1);
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
		if (curHealth > 0 + loss) {
			
			if(curHealth > 1) {
				audio.PlayOneShot (shieldHitSound);
			}
			else {
				audio.PlayOneShot(shieldDownSound);
			}
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
	
	#region #4 Player Powers
	
	void ChangeColors (ColorWheel color)
	{
		playerRGB.color = color;
		pickups = GameObject.FindGameObjectsWithTag (Tags.PICKUP);
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
							block.GetComponent<BlockLogic> ().BlowUp ();
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
}