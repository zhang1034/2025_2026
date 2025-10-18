using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AA0000
{
	public class Spawner : MonoBehaviour
	{
		public RingAreaUsingCollider locationProvider;
		public GameObject spanwnedObject;
		public int amountOfSpawnedThings = 50;
		public bool spawnEndlessly = false;
		public float spawnRate = 0.1f;
		public List<GameObject> spawnedObjects;
		bool canWeSpawn = true;

		private void Awake()
		{
			locationProvider = GetComponent<RingAreaUsingCollider>();
		}

		// Update is called once per frame
		void Update()
		{
			if (spanwnedObject != null)
			{
				if ((spawnEndlessly || amountOfSpawnedThings > 0) && canWeSpawn)
				{
					StartCoroutine("SpawnObject");
				}
			}
		}

		IEnumerator SpawnObject()
		{
			canWeSpawn = !canWeSpawn;
			yield return new WaitForSecondsRealtime(spawnRate);
			if (!spawnEndlessly)
				amountOfSpawnedThings--;
			Vector3 spawnLocation = locationProvider.GetRandomPositionInRing();
			spawnedObjects.Add(Instantiate(spanwnedObject, spawnLocation, Quaternion.identity));
			canWeSpawn = !canWeSpawn;
		}
	}
}
