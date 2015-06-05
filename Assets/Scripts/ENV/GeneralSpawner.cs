using UnityEngine;
using System.Collections;

public class GeneralSpawner : MonoBehaviour {

	//[System.Serializable]
	public class SpawnedObject{
		public GameObject gameObject;
		public float spawnTime = 0.0f;
	}

	public float timeToSpawn = 4.0f;
	public GameObject objectToSpawn;

	public bool destroyAfterTime = true;
	public float timeToDestroy = 20.0f;

	float lastIstantiatedTime = 0.0f;

	public bool spawnOnStart = true;

	//[SerializeField]
	public SpawnedObject[] spawnedObjects = new SpawnedObject[30];

	void Start () {
		if (objectToSpawn == null)
			Debug.Log ("ATTENZIONE!!! Nessun oggetto da spawnare in "+ gameObject.name);

		if (spawnOnStart)
			instantiateAfterTimeMethod();
	}

	void Update () {
		if (destroyAfterTime)
			destroyAfterTimeMethod();
		if (Mathf.Abs(Time.time-lastIstantiatedTime) > timeToSpawn)
			instantiateAfterTimeMethod();
	}

	void destroyAfterTimeMethod()
	{
		for (int i = 0; i<spawnedObjects.Length; i++)
		{
			if (spawnedObjects[i] != null)
			{
				if (Mathf.Abs(Time.time - spawnedObjects[i].spawnTime) > timeToDestroy)
				{
					Destroy(spawnedObjects[i].gameObject);
					spawnedObjects[i] = null;
				}
			}
		}
	}

	void instantiateAfterTimeMethod()
	{
		int index = firstNullIndex();
		spawnedObjects[index] = new SpawnedObject();
		spawnedObjects[index].gameObject = Instantiate(objectToSpawn);
		spawnedObjects[index].spawnTime = Time.time;

		//setto la posizione corretta
		spawnedObjects[index].gameObject.transform.parent = transform;
		spawnedObjects[index].gameObject.transform.localPosition = Vector3.zero;

		lastIstantiatedTime = Time.time;
	}

	int firstNullIndex()
	{
		for (int i = 0; i<spawnedObjects.Length; i++)
		{
			if (spawnedObjects[i] == null || spawnedObjects[i].gameObject == null)
			{
				//Debug.Log ("trovato nullo " +i);
				return i;
			}
		}
		return 0;
	}
}
