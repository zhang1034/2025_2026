using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveUnseen : MonoBehaviour
{
	public Camera cameraToTrack;
	public Plane[] frustrumPlanes;
	public Collider objectColliderToCheck;

	void OnEnable()
	{
		print("we are enabled");
		cameraToTrack = Camera.main;
		objectColliderToCheck = GetComponent<Collider>();
	}

	private void FixedUpdate()
	{
		if (!CheckIfInsideFrustrumPlanes())
		{
			print("destroying!");
			Destroy(gameObject);
		}
	}

	private bool CheckIfInsideFrustrumPlanes()
	{
		frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(cameraToTrack);
		if (GeometryUtility.TestPlanesAABB(frustrumPlanes, objectColliderToCheck.bounds))
		{
			Debug.Log("I am inside the camera frustrum!");
			return true;
		}
		else
		{
			Debug.Log("I am out of sight...");
			return false;
		}

	}

	// Another possibility, but this will check if in view of ANY camera
	private void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}
