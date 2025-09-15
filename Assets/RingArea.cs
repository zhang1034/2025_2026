using UnityEngine;

namespace AA0000
{
	[RequireComponent(typeof(SphereCollider))]
	public class RingAreaUsingCollider : MonoBehaviour
	{
		private SphereCollider sphereCollider;

		// Inner radius as a percentage of the SphereCollider's radius (0 to 1)
		[Range(0f, 1f)]
		public float innerRadiusPercentage = 0.5f;

		void Awake()
		{
			// Get the attached SphereCollider
			sphereCollider = GetComponent<SphereCollider>();
		}

		void Start()
		{
			// Example usage
			Vector3 randomPosition = GetRandomPositionInRing();
			Debug.Log("Random Position in Ring: " + randomPosition);
		}

		// Method to get a random position inside the ring area
		public Vector3 GetRandomPositionInRing()
		{
			// Get the outer and inner radii based on the SphereCollider's radius
			float outerRadius = sphereCollider.radius * transform.localScale.x;  // Account for object scaling
			float innerRadius = outerRadius * innerRadiusPercentage;

			// Generate a random angle in radians
			float angle = Random.Range(0f, Mathf.PI * 2);

			// Generate a random radius between inner and outer limits
			float radius = Mathf.Sqrt(Random.Range(innerRadius * innerRadius, outerRadius * outerRadius));

			// Calculate the X and Z components based on the angle and radius
			float x = transform.position.x + radius * Mathf.Cos(angle);
			float z = transform.position.z + radius * Mathf.Sin(angle);

			// Return the new position on the XZ plane (keeping Y position at the object's level)
			return new Vector3(x, transform.position.y, z);
		}
	} 
}
