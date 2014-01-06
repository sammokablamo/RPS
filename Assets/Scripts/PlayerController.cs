using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour

{
	//Ouya Controller variables
	//private const float INNER_DEADZONE = 0.3f;
	
	private float baseMoveSpeed;
	private float maxMoveSpeed;
	private float moveSpeedDissipationRate;
	public float currentMoveSpeed;
	private float speedIncrementPerPill;

	//player speed
	//public float speed; 

	private InputHandler inputHandler;//reference input handler, create a local variable version of class input handler
	//reference GameManager's Gameobject
	public GameObject gameManagerGameObject;
	private GameManager gameManager; //localprivate reference to game object script

	//create array of meshes
	public Mesh rockMesh;
	public Mesh paperMesh;
	public Mesh scissorMesh;

	//reference for mesh filter
	private MeshFilter meshFilter;
	private MeshFilter otherMeshFilter;

	//reference for other colliding player's player number enum
	private InputHandler otherInputHandler;
	private bool classSelectState;

	//setup audio references
	public AudioClip RockKillClip;      // Audio clip of the rock kill
	public AudioClip PaperKillClip;      // Audio clip of the rock kill
	public AudioClip ScissorsKillClip;      // Audio clip of the rock kill
	public AudioClip PickupPillClip;
	enum PlayerAudioFX {RockKillAudio, PaperKillAudio, ScissorKillAudio, PickupPillAudio};

	void Awake() //Awake is called when the script instance is being loaded.
	{
		//setup references
		inputHandler = GetComponent<InputHandler> (); //gamepad input handler script reference
		gameManagerGameObject = GameObject.Find("GameManager");//this will handle when class switching can occur
		gameManager = gameManagerGameObject.GetComponent<GameManager>();

		//mesh filter component reference
		meshFilter = GetComponent<MeshFilter> ();

		baseMoveSpeed = gameManager.baseMoveSpeed;
		maxMoveSpeed = gameManager.maxMoveSpeed;
		moveSpeedDissipationRate = gameManager.moveSpeedDissipationRate;
		speedIncrementPerPill = gameManager.speedIncrementPerPill;

	}

	void OnDestroy()//This function is called when the MonoBehaviour will be destroyed. Cleanup Ouya components on destroyed
	{
	}


	//start is called on the frame when a scrip tis enabled just before any of the update methods are called for the first time.
	void Start ()
	{


	}

	//Update is called every frame, if the MonoBehaviour is enabled.
	void Update ()
	{

		currentMoveSpeed = Mathf.Lerp (currentMoveSpeed, baseMoveSpeed, moveSpeedDissipationRate* Time.deltaTime);
		//Debug.Log ( "currentMoveSpeed " + currentMoveSpeed);
		classSelectionButtonInterations ();

	}
	//fixed update is called every fixed framerate frame if monobehavior is enabled.
	void FixedUpdate ()
	{
		//player controls
		float moveHorizontal = inputHandler.x_Axis_LeftStick; //connect variables with external references
		float moveVertical = inputHandler.y_Axis_LeftStick;


		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rigidbody.AddForce(movement * currentMoveSpeed * Time.deltaTime);

		//pickup stuff
		//when player controller collides with an object, create a collider variable and name it "other".


	}

	void OnTriggerEnter(Collider other) 
	{
		//Debug.Log ("How many times have i triggered this");
		//only do trigger stuff if classSelectState is false
		if (gameManager.ClassSelectState == false)
		{
		//if the collider variable named other has a tag that's equal to "pickup" then...
			if(other.gameObject.tag == "PickUp")
			{
				//set active to false, turning it off.
				other.gameObject.SetActive(false);
				AudioManagement(PlayerAudioFX.PickupPillAudio);
				currentMoveSpeed = currentMoveSpeed + speedIncrementPerPill;
				//cap move speed
				if (currentMoveSpeed > maxMoveSpeed)
				{
					currentMoveSpeed = maxMoveSpeed;
				}

				gameManager.startPickupRespawnTimerProxy(other.gameObject);
				//Debug.Log ("got past pickup respawn method call");
			}
			
			if (other.gameObject.tag == "Player")
			{
				
				otherMeshFilter = other.gameObject.GetComponent<MeshFilter>(); //set reference to Mesh Filter component of other game object.
				otherInputHandler = other.gameObject.GetComponent<InputHandler>();
				
				if (meshFilter.mesh.ToString() == otherMeshFilter.mesh.ToString()) //Debug.Log ("Are we the same? " + same, gameObject);
				{
					//Debug.Log ("We're the same, do nothing.", gameObject);
				}
				if (meshFilter.mesh.ToString() == "Rock Instance (UnityEngine.Mesh)" && otherMeshFilter.mesh.ToString() == "Scissors Instance (UnityEngine.Mesh)") //Am I a rock and the other thing is scissor.this may not be the best way but it works...
				{
					other.gameObject.SetActive(false); //turn off other object
					gameManager.addToPlayerScore(inputHandler.player, 1, otherInputHandler.player); //add to player score and tell manager who died.
					AudioManagement(PlayerAudioFX.RockKillAudio);
					//Debug.Log(inputHandler.player);
					//Debug.Log ("Killed a scissor", gameObject);
				}

				if (meshFilter.mesh.ToString() == "Paper Instance (UnityEngine.Mesh)" && otherMeshFilter.mesh.ToString() == "Rock Instance (UnityEngine.Mesh)" ) //Am I Paper and is the other thing Rock?
				{
					other.gameObject.SetActive(false); //turn off other object
					gameManager.addToPlayerScore(inputHandler.player, 1, otherInputHandler.player); //add to player score and tell manager who died.
					AudioManagement(PlayerAudioFX.PaperKillAudio);
					//Debug.Log ("Killed a rock", gameObject);
				}
				if (meshFilter.mesh.ToString() == "Scissors Instance (UnityEngine.Mesh)" && otherMeshFilter.mesh.ToString() == "Paper Instance (UnityEngine.Mesh)" ) //Am I scissors and is the other thing a paper instance?
				{
					other.gameObject.SetActive(false); //turn off other object
					gameManager.addToPlayerScore(inputHandler.player, 1, otherInputHandler.player); //add to player score and tell manager who died.
					AudioManagement(PlayerAudioFX.ScissorKillAudio);
					Debug.Log ("Killed a paper", gameObject);
				}
			}
		}
	}

	void AudioManagement (PlayerAudioFX AudioToPlay)
	{

		// If the shout input has been pressed...
		if(AudioToPlay == PlayerAudioFX.RockKillAudio)
		{
			// ... play the shouting clip where we are.
			AudioSource.PlayClipAtPoint(RockKillClip, transform.position);
			//Debug.Log("play rock audio");
		}
		else if (AudioToPlay == PlayerAudioFX.PaperKillAudio)
		{
			AudioSource.PlayClipAtPoint(PaperKillClip, transform.position);
			//Debug.Log("play paper audio");
		}
		else if (AudioToPlay == PlayerAudioFX.ScissorKillAudio)
		{
			AudioSource.PlayClipAtPoint(ScissorsKillClip, transform.position);
			//Debug.Log("play scissor audio");
		}
		else if (AudioToPlay == PlayerAudioFX.PickupPillAudio)
		{
			AudioSource.PlayClipAtPoint(PickupPillClip, transform.position);
			//Debug.Log("play pill audio");
		}
	}


	void SetCountText()
	{
		//countText.text = "Count: " + count.ToString ();
		//if (count >= 16) 
		//{
		//	winText.text = "YOU WIN!";
		//}
	}

	void classSelectionButtonInterations ()
	{
		//Debug.Log ("Update", gameObject);
		bool rockButton = inputHandler.down_U;
		bool paperButton = inputHandler.down_Y;
		bool scissorButton = inputHandler.down_A;
		// class selection
		if (gameManager.ClassSelectState == true) {
			if (rockButton == true) {
				//Debug.Log ("Rock", gameObject);
				meshFilter.mesh = rockMesh;
				//SetClass(OuyaPlayer, 0); //A yet uncreated function that sets current player to a class number, rock
			}
			else
				if (paperButton == true) {
					Debug.Log ("Paper", gameObject);
					meshFilter.mesh = paperMesh;
					//SetClass(OuyaPlayer, 1);
				}
				else
					if (scissorButton == true) {
						Debug.Log ("Scissor", gameObject);
						meshFilter.mesh = scissorMesh;
						//SetClass(OuyaPlayer, 2);
					}
					else {
						//Debug.Log ("no button conditions are true", gameObject);
					}
		}
		else {
			//Debug.Log ("Class Select State is false", gameObject);
		}
	}

	public void OuyaMenuButtonUp()
	{
	}
	
	public void OuyaMenuAppearing()
	{
	}
	
	public void OuyaOnPause()
	{
	}
	
	public void OuyaOnResume()
	{
	}

}
