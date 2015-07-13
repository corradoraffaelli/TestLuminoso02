using UnityEngine;
using System.Collections;

public class GeneralSpawner : MonoBehaviour {

	//[System.Serializable]
	public class SpawnedObject{
		public GameObject gameObject;
		public float spawnTime = 0.0f;
		public bool toFlash = false;
		public bool toReachMin = true;
		public SpriteRenderer[] spriteRenderers;
	}

	public float timeToSpawn = 4.0f;
	public GameObject objectToSpawn;

	public bool destroyAfterTime = true;
	public float timeToDestroy = 20.0f;

	public float timeAfterFlash = 3.0f;
	public float flashSpeed = 2.0f;
	public bool flashBlack = true;
	public bool flashAlpha = false;
	[Range(0.0f, 1.0f)]
	public float minColorAlpha = 0.5f;

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

		setNeedToFlash();
		manageFlashes();
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

		spawnedObjects[index].spriteRenderers = spawnedObjects[index].gameObject.GetComponentsInChildren<SpriteRenderer>();

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

	void setBalloonAlpha(float inputAlpha, SpriteRenderer[] inputRenderers)
	{
		if (inputRenderers != null)
		{
			for (int i = 0; i < inputRenderers.Length; i++)
			{
				if (inputRenderers[i] != null)
				{
					Color tempColor = inputRenderers[i].color;
					inputRenderers[i].color = new Color(tempColor.r, tempColor.g, tempColor.b, inputAlpha);
				}
			}
		}
	}

	void setBalloonColor(float inputColor, SpriteRenderer[] inputRenderers)
	{
		if (inputRenderers != null)
		{
			for (int i = 0; i < inputRenderers.Length; i++)
			{
				if (inputRenderers[i] != null)
				{
					Color tempColor = inputRenderers[i].color;
					inputRenderers[i].color = new Color(inputColor, inputColor, inputColor, tempColor.a);
				}
			}
		}
	}

	void setNeedToFlash()
	{
		if (spawnedObjects != null)
		{
			for (int i = 0; i < spawnedObjects.Length; i++)
			{
				if (spawnedObjects[i] != null && !spawnedObjects[i].toFlash && spawnedObjects[i].gameObject != null)
				{
					if ((timeToDestroy - (Time.time - spawnedObjects[i].spawnTime)) < timeAfterFlash)
					{
						//Debug.Log ("mongolfiera "+i+" sta per essere distrutta");
						spawnedObjects[i].toFlash = true;
					}
						
				}
			}
		}
	}

	void manageFlashes()
	{
		if (spawnedObjects != null)
		{
			for (int i = 0; i < spawnedObjects.Length; i++)
			{
				if (spawnedObjects[i] != null && spawnedObjects[i].toFlash && spawnedObjects[i].gameObject != null && spawnedObjects[i].spriteRenderers != null)
				{
					if (spawnedObjects[i].toReachMin)
					{
						if (flashAlpha && spawnedObjects[i].spriteRenderers[0] != null)
						{
							float tempAlpha = spawnedObjects[i].spriteRenderers[0].color.a;
							float newAlpha = Mathf.MoveTowards(tempAlpha, minColorAlpha, Time.deltaTime * flashSpeed);
							setBalloonAlpha(newAlpha, spawnedObjects[i].spriteRenderers);
							if (newAlpha == minColorAlpha)
								spawnedObjects[i].toReachMin = false;
						}
						if (flashBlack && spawnedObjects[i].spriteRenderers[0] != null)
						{
							float tempColor = spawnedObjects[i].spriteRenderers[0].color.r;
							float newColor = Mathf.MoveTowards(tempColor, minColorAlpha, Time.deltaTime * flashSpeed);
							setBalloonColor(newColor, spawnedObjects[i].spriteRenderers);
							if (newColor == minColorAlpha)
								spawnedObjects[i].toReachMin = false;
						}
					}
					else
					{
						if (flashAlpha && spawnedObjects[i].spriteRenderers[0] != null)
						{
							float tempAlpha = spawnedObjects[i].spriteRenderers[0].color.a;
							float newAlpha = Mathf.MoveTowards(tempAlpha, 1.0f, Time.deltaTime * flashSpeed);
							setBalloonAlpha(newAlpha, spawnedObjects[i].spriteRenderers);
							if (newAlpha == 1.0f)
								spawnedObjects[i].toReachMin = true;
						}
						if (flashBlack && spawnedObjects[i].spriteRenderers[0] != null)
						{
							float tempColor = spawnedObjects[i].spriteRenderers[0].color.r;
							float newColor = Mathf.MoveTowards(tempColor, 1.0f, Time.deltaTime * flashSpeed);
							setBalloonColor(newColor, spawnedObjects[i].spriteRenderers);
							if (newColor == 1.0f)
								spawnedObjects[i].toReachMin = true;
						}
					}
				}
			}
		}
	}
}
