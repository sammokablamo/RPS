using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;

	//gui text variable
	public GUIText countText;
	public GUIText winText;

	//counter
	private int count;
	//start is called on the frame when a scrip tis enabled just before any of the update methods are called for the first time.
	void Start ()
	{
		count = 0;
		SetCountText();
		winText.text = "";
	}

	//fixed update is called every fixed framerate frame if monobehavior is enabled.
	void FixedUpdate ()
	{
		//player controls
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rigidbody.AddForce(movement * speed * Time.deltaTime);

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

}
