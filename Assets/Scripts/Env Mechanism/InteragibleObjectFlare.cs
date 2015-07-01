using UnityEngine;
using System.Collections;

public class InteragibleObjectFlare : MonoBehaviour {

	public GameObject flarePrefab;
	public Transform flarePosition;

	[Range(0.0f, 15.0f)]
	public float distanceLimitPlayer = 6.0f;
	public bool onlyIfPlayerNear = true;

	SpriteRenderer spriteRenderer;

	TimeSyncronizerInteragible sync;
	GameObject player;
	Transform playerTransform;

	bool timeToFlare = false;
	bool needToPulse = false;
	bool wasNeedToPulse = false;

	//public bool nearPlayer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();

		GameObject controller = GeneralFinder.controller;
		if (controller != null)
			sync = controller.GetComponent<TimeSyncronizerInteragible>();

		player = GeneralFinder.player;
		if (player != null)
			playerTransform = player.GetComponent<Transform>();

		if (!onlyIfPlayerNear)
			distanceLimitPlayer = 15.0f;
	}

	void Update () {
		if (spriteRenderer.enabled && spriteRenderer.color.a != 0.0f)
		{
			needToPulse = sync.NeedToPulse;
			if (needToPulse && !wasNeedToPulse)
			{
				timeToFlare = true;
			}
			wasNeedToPulse = needToPulse;
			
			if (timeToFlare)
			{
				if (isPlayerNear())
					Instantiate(flarePrefab, flarePosition.position, Quaternion.identity);
			}
			
			//nearPlayer = isPlayerNear();
			
			timeToFlare = false;
		}

	}

	bool verifyIfNull()
	{
		if (flarePrefab != null && flarePosition != null)
			return true;
		else
			return false;
	}

	bool isPlayerNear()
	{
		if (Vector3.Distance(playerTransform.position, transform.position) < distanceLimitPlayer)
			return true;
		else 
			return false;
	}
}
