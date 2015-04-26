using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour {

	GameObject player;
	Collider2D[] playerColliders;
	Collider2D platformCollider;
	Rigidbody2D rigidBody;
	SpriteRenderer spriteRenderer;
	float maxFallingSpeed;

	bool playerWasOver = false;
	bool playerOver = false;
	float sogliaControllo = 0.0f;
	//public float maxSogliaControllo = 0.6f;
	//bool playerWasBelow = false;
	//bool needToIgnore = false;
	//bool needToNotIgnore = false;
	int toIgnoreProgressive = 0;

	public bool debugVariables = false;
	public int threshBeforeIgnore = 10;
	public bool useSpriteLimits = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerColliders = player.GetComponents<Collider2D>();
		platformCollider = transform.GetComponent<Collider2D> ();
		rigidBody = player.GetComponent<Rigidbody2D>();
		maxFallingSpeed = player.GetComponent<PlayerMovements> ().maxFallingSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (useSpriteLimits) {
			spriteRenderer = player.GetComponent<SpriteRenderer> ();

			float playerLimit = spriteRenderer.bounds.min.y;
			float platLimit = platformCollider.bounds.max.y;
			if (playerLimit >= platLimit && !playerWasOver)
			{
				playerWasOver = true;
				changeIgnoreCollider(false);
			}
			else if (playerLimit < platLimit && playerWasOver)
			{
				playerWasOver = false;
				changeIgnoreCollider(true);
			}
		}
	}

	void FixedUpdate()
	{
		if (!useSpriteLimits) {
			playerOver = isPlayerOver (getPlayerBottomCollider (), platformCollider);
			if (playerOver)
				toIgnoreProgressive = 0;
			//Debug.Log (playerOver);
			if (playerOver && !playerWasOver) {
				changeIgnoreCollider (false);
				playerWasOver = true;
				if (debugVariables)
					Debug.Log ("da settare");
			}
			
			if (!playerOver && playerWasOver && toIgnoreProgressive < threshBeforeIgnore) {
				toIgnoreProgressive ++;
				if (debugVariables)
					Debug.Log (toIgnoreProgressive);
			}
			
			if (!playerOver && playerWasOver && toIgnoreProgressive == threshBeforeIgnore) {
				changeIgnoreCollider(true);
				playerWasOver = false;
				toIgnoreProgressive = 0;
				if (debugVariables)
					Debug.Log ("da ignorare");
			}
		}

	}

	//ritorna il collider con la y più bassa
	Collider2D getPlayerBottomCollider()
	{
		bool primo = true;
		float limit = 0.0f;
		Collider2D basso= null;
		for (int i = 0; i< playerColliders.Length; i++) {
			if (primo)
			{
				limit = playerColliders[i].bounds.min.y;
				basso = playerColliders[i];
				primo = true;
			}else{
				if (playerColliders[i].bounds.min.y < limit)
				{
					limit = playerColliders[i].bounds.min.y;
					basso = playerColliders[i];
				}
			}
		}
		return basso;
	}

	bool isPlayerOver(Collider2D playerCollider, Collider2D platCollider)
	{
		//Debug.Log (platCollider.bounds.max.y - platCollider.bounds.min.y);
		//se il player sta cadendo, aumento la soglia
		if (rigidBody.velocity.y < 0.0f) {
			//sogliaControllo = maxSogliaControllo *(Mathf.Abs (rigidBody.velocity.y)) / maxFallingSpeed;
			sogliaControllo = Time.fixedDeltaTime * (Mathf.Abs (rigidBody.velocity.y)) * 2;

		} else {
			sogliaControllo = 0.0f;
		}
		//Debug.Log (sogliaControllo);

		float playerLimit = playerCollider.bounds.min.y;
		float platLimit = platCollider.bounds.max.y;
		if (playerLimit > (platLimit-sogliaControllo))
			return true;
		else
			return false;
	}

	void changeIgnoreCollider(bool ignoreOrNot = true)
	{
		for (int i = 0; i< playerColliders.Length; i++) {
			Physics2D.IgnoreCollision(playerColliders[i], platformCollider, ignoreOrNot);
		}
	}
}
