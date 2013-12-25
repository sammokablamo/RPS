using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
/*	//Ouya references, testing whether i still need this.
	OuyaSDK.IMenuButtonUpListener,
	OuyaSDK.IMenuAppearingListener,
	OuyaSDK.IPauseListener,
	OuyaSDK.IResumeListener
*/
{
	//Ouya Controller variables
	//private const float INNER_DEADZONE = 0.3f;
	
	private const float MOVE_SPEED = 200f;
	
	//public OuyaSDK.OuyaPlayer Index;

	//player speed
	//public float speed; 

	//gui text variable
	public GUIText countText;
	public GUIText winText;

	//counter
	private int count;

	private InputHandler inputHandler;//reference input handler, create a local variable version of class input handler

	void Awake() //Awake is called when the script instance is being loaded.
	{
		/*
		OuyaSDK.registerMenuButtonUpListener(this);
		OuyaSDK.registerMenuAppearingListener(this);
		OuyaSDK.registerPauseListener(this);
		OuyaSDK.registerResumeListener(this);
		Input.ResetInputAxes();
		*/

		//setup references
		inputHandler = GetComponent<InputHandler> ();
	}

	void OnDestroy()//This function is called when the MonoBehaviour will be destroyed. Cleanup Ouya components on destroyed
	{
		/*
		OuyaSDK.unregisterMenuButtonUpListener(this);
		OuyaSDK.unregisterMenuAppearingListener(this);
		OuyaSDK.unregisterPauseListener(this);
		OuyaSDK.unregisterResumeListener(this);
		Input.ResetInputAxes();
		*/
}


	//start is called on the frame when a scrip tis enabled just before any of the update methods are called for the first time.
	void Start ()
	{
		count = 0;
		SetCountText();
		winText.text = "";
	}

	//Update is called every frame, if the MonoBehaviour is enabled.
	void Update ()
	{

	}
	//fixed update is called every fixed framerate frame if monobehavior is enabled.
	void FixedUpdate ()
	{
		//player controls
		float moveHorizontal = inputHandler.x_Axis_LeftStick;
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
