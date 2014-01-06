using UnityEngine;
using System.Collections;

public class PlayerNumberLight : MonoBehaviour {
	public GameObject playerGameObject;
	private Vector3 newPosition;
	// Use this for initialization
	void Start () {
	//setup reference to player

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		newPosition = new Vector3(playerGameObject.GetComponent<Transform>().position.x, playerGameObject.GetComponent<Transform>().position.y + 1.0f, playerGameObject.GetComponent<Transform>().position.z);
		transform.position = newPosition;
		if (playerGameObject.activeSelf == false)
		{
			light.intensity = 0;
		}
		if (playerGameObject.activeSelf == true)
		{
			light.intensity = 4;
		}
		//transform.LookAt( Vector3.up, Vector3.up);
	}
}
