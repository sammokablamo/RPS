using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	//smooth lookat declarations
	private Vector3 cameraTarget;
	private Vector3 cameraRelativePosition; //relative position of camera from lookat target
	public float cameraLookatSpeed;



	//position declarations
	private float newCameraPositionX;
	public float cameraRepositionSpeed;
	// Update is called once per frame
	public GameManager gameManager;

	//limits on rotation and position
	public Vector3 PositionMinLimit;
	public Vector3 PositionMaxLimit;
	private Vector3 PositionLimitProxy;

	public Vector3 RotationMinLimit;
	public Vector3 RotationMaxLimit;
	private Quaternion RotationLimitProxy;
	private Vector3 RotationOffsetForEulerCalc;

	public float averageLocationOffsetZ;

	void Start ()
	{

		gameManager = gameManager.GetComponent<GameManager>(); //setup reference to game manager
		RotationOffsetForEulerCalc = new Vector3 (360,360,360);
		RotationMinLimit = RotationMinLimit + RotationOffsetForEulerCalc;
		RotationMaxLimit = RotationMaxLimit + RotationOffsetForEulerCalc;
	}

	//LateUpdate is called after all Update functions have been called. This is useful to order script execution. For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.

	void FixedUpdate () 
	{
		gameManager.setLiveAndDeadPlayerArrays ();
		// Create a vector from the camera towards the average position of live players.
		cameraTarget = gameManager.findAverageLocationOfLivePlayers ();//transform.position = player.transform.position + offset;

		cameraTarget.y = cameraTarget.y * .1f; //keep x and z closer to 0
		cameraTarget.z = cameraTarget.z * .1f + averageLocationOffsetZ;

		// Lerp the camera's position between it's current position and it's new position.
		//ONLY TAKE THE X position of target so it slides on a cable like a football field camera
		newCameraPositionX = Vector3.Lerp(transform.position, cameraTarget, cameraRepositionSpeed * Time.deltaTime).x;

		newCameraPositionX = Mathf.Clamp(newCameraPositionX, PositionMinLimit.x, PositionMaxLimit.x);
		transform.position = new Vector3(newCameraPositionX, transform.position.y, transform.position.z);
		//SmoothLookAt();	


	}

	void SmoothLookAt ()
	{

		cameraRelativePosition = cameraTarget - transform.position;

		// Create a rotation based on the relative position of the player being the forward vector.
		Quaternion lookAtRotation = Quaternion.LookRotation(cameraRelativePosition, Vector3.up);
		
		// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
		transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, cameraLookatSpeed * Time.deltaTime);

		limitCameraRotation ();

		transform.rotation = Quaternion.Lerp(transform.rotation, RotationLimitProxy, cameraLookatSpeed * Time.deltaTime);


	}

	void limitCameraRotation ()
	{
		//assign transform.rotatation quat to a temp working quat variable
		RotationLimitProxy = transform.rotation;
		//clamp camera rotation between limits
		RotationLimitProxy = Quaternion.Euler (RotationLimitProxy.eulerAngles + RotationOffsetForEulerCalc);

		RotationLimitProxy = Quaternion.Euler (
			Mathf.Clamp (RotationLimitProxy.eulerAngles.x, RotationMinLimit.x, RotationMaxLimit.x), 
			Mathf.Clamp (RotationLimitProxy.eulerAngles.y, RotationMinLimit.y, RotationMaxLimit.y), 
			Mathf.Clamp (RotationLimitProxy.eulerAngles.z, RotationMinLimit.z, RotationMaxLimit.z)
			);
		/*
		//keep range within 0 to 360 so it's quat friendly
		while (RotationLimitProxy.eulerAngles.x >= 360) {
			RotationLimitProxy.x = RotationLimitProxy.x - RotationOffsetForEulerCalc.x;
		}
		while (RotationLimitProxy.eulerAngles.y >= 360) {
			RotationLimitProxy.y = RotationLimitProxy.y - RotationOffsetForEulerCalc.y;
		}
		while (RotationLimitProxy.eulerAngles.z >= 360) {
			RotationLimitProxy.z = RotationLimitProxy.z - RotationOffsetForEulerCalc.z;
		}
		*/
	}
}
