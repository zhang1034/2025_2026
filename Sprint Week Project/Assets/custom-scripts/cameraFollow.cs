// credit: https://github.com/coderDarren/Unity3D-Cars

using UnityEngine;

public class cameraFollow : MonoBehaviour
{
  public void LookAtTarget()
	{
		Vector3 _lookDirection = objectToFollow.position - transform.position;
		Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
	}

	public void MoveToTarget()
	{
		Vector3 _targetPos = objectToFollow.position +
							 objectToFollow.forward * offset.z +
							 objectToFollow.right * offset.x +
							 objectToFollow.up * offset.y;
		transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
	}

	private void FixedUpdate()
	{
		LookAtTarget();
		MoveToTarget();
	}

	// Getting a reference of the car
	public Transform objectToFollow;
	// Stores 3 floats (x, y, z) to offset the camera from the car
	public Vector3 offset;
	public float followSpeed = 10;
	public float lookSpeed = 10;
}
