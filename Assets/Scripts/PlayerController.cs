using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour

{
	//Ouya Controller variables
	//private const float INNER_DEADZONE = 0.3f;
	
	private const float MOVE_SPEED = 100f;
	
	//public OuyaSDK.OuyaPlayer Index;

	//player speed
	//public float speed; 

	//gui text variable
	public GUIText countText;
	public GUIText winText;

	//counter
	private int count;

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

	void Awake() //Awake is called when the script instance is being loaded.
	{
		//setup references
		inputHandler = GetComponent<InputHandler> (); //gamepad input handler script reference
		gameManagerGameObject = GameObject.Find("GameManager");//this will handle when class switching can occur
		gameManager = gameManagerGameObject.GetComponent<GameManager>();

		//mesh filter component reference
		meshFilter = GetComponent<MeshFilter> ();


	}

	void OnDestroy()//This function is called when the MonoBehaviour will be destroyed. Cleanup Ouya components on destroyed
	{
	}


	//start is called on the frame when a scrip tis enabled just before any of the update methods are called for the first time.
	void Start ()
	{

		//bool classSelectState = gameManager.GetComponent<GameManager>().ClassSelectState;

		count = 0;
		SetCountText();
		winText.text = "";
	}

	//Update is called every frame, if the MonoBehaviour is enabled.
	void Update ()
	{
		//Debug.Log ("Update", gameObject);
		bool rockButton = inputHandler.down_U;
		bool paperButton = inputHandler.down_Y;
		bool scissorButton = inputHandler.down_A;
/*
		GUI.Label(new Rect(50, 260, 100, 20), "O " + OuyaInput.GetButton(OuyaButton.O, player));
		GUI.Label(new Rect(50, 280, 100, 20), "U " + OuyaInput.GetButton(OuyaButton.U, player));
		GUI.Label(new Rect(50, 300, 100, 20), "Y " + OuyaInput.GetButton(OuyaButton.Y, player));
		GUI.Label(new Rect(50, 320, 100, 20), "A " + OuyaInput.GetButton(OuyaButton.A, player));
		*/

		// class selection
		if (gameManager.ClassSelectState == true) 
		{
			if (rockButton == true)
			{
				Debug.Log ("Rock", gameObject);
				meshFilter.mesh = rockMesh;
				//SetClass(OuyaPlayer, 0); //A yet uncreated function that sets current player to a class number, rock
			
			}
			else if (paperButton == true)
			{
				Debug.Log ("Paper", gameObject);
				meshFilter.mesh = paperMesh;
				//SetClass(OuyaPlayer, 1);
			}
			else if (scissorButton == true)
			{
				Debug.Log ("Scissor", gameObject);
				meshFilter.mesh = scissorMesh;
				//SetClass(OuyaPlayer, 2);
			}
			else
			{
				//Debug.Log ("no button conditions are true", gameObject);
			}
			
		} else{
			Debug.Log ("Class Select State is false", gameObject);
		}
	}
	//fixed update is called every fixed framerate frame if monobehavior is enabled.
	void FixedUpdate ()
	{
		//player controls
		float moveHorizontal = inputHandler.x_Axis_LeftStick; //connect variables with external references
		float moveVertical = inputHandler.y_Axis_LeftStick;


		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rigidbody.AddForce(movement * MOVE_SPEED * Time.deltaTime);

		//pickup stuff
		//when player controller collides with an object, create a collider variable and name it "other".


	}

	void OnTriggerEnter(Collider other) 
	{
		//if the collider variable named other has a tag that's equal to "pickup" then...
		if(other.gameObject.tag == "PickUp")
		{
			//set active to false, turning it off.
			other.gameObject.SetActive(false);
			//increment count by one.
			count = count + 1;
			SetCountText();
			//set active to false, turning it off.
			other.gameObject.SetActive(false);
			//increment count by one.
			count = count + 1;
			SetCountText();
			Debug.Log ("PickedStuffUp", gameObject);
		}

		if (other.gameObject.tag == "Player")
		{
		
			otherMeshFilter = other.gameObject.GetComponent<MeshFilter>();
			Debug.Log ("What's in RockMeshTostring? " + rockMesh.ToString(), gameObject);
			//Debug.Log ("Hit a " + otherMeshFilter.mesh.ToString(), gameObject);
			Debug.Log ("I am a " + meshFilter.mesh.ToString() + "EndOfString", gameObject); 
	
			bool same = meshFilter.mesh.ToString() == otherMeshFilter.mesh.ToString();

			if (meshFilter.mesh.ToString() == otherMeshFilter.mesh.ToString()) //Debug.Log ("Are we the same? " + same, gameObject);
			{
				Debug.Log ("We're the same, do nothing.", gameObject);
			}
			if (meshFilter.mesh.ToString() == "Rock Instance (UnityEngine.Mesh)") //this may not be the best way but it works...
			{
				//Debug.Log ("I'm a rock.", gameObject);
				if (otherMeshFilter.mesh.ToString() == "Scissors Instance (UnityEngine.Mesh)")
				{
					//Debug.Log ("...and the other thing is scissor.", gameObject);
					other.gameObject.SetActive(false);
					Debug.Log ("Killed a scissor", gameObject);
				}
			}
			if (meshFilter.mesh.ToString() == "Scissors Instance (UnityEngine.Mesh)" && otherMeshFilter.mesh.ToString() == "Paper Instance (UnityEngine.Mesh)" ) 
			{
				other.gameObject.SetActive(false);
				Debug.Log ("Killed a paper", gameObject);
			}
			if (meshFilter.mesh.ToString() == "Paper Instance (UnityEngine.Mesh)" && otherMeshFilter.mesh.ToString() == "Rock Instance (UnityEngine.Mesh)" ) 
			{
				other.gameObject.SetActive(false);
				Debug.Log ("Killed a rock", gameObject);
			}

		}
	}

	void SetCountText()
	{
		countText.text = "Count: " + count.ToString ();
		if (count >= 16) 
		{
			winText.text = "YOU WIN!";
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
