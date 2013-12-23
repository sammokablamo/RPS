using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject player;
	private Vector3 offset;
	// Update is called once per frame

	void Start ()
	{
		offset = transform.position;
	}

	//LateUpdate is called after all Update functions have been called. This is useful to order script execution. For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.

	void LateUpdate () 
	{
		transform.position = player.transform.position + offset;
	}
}
