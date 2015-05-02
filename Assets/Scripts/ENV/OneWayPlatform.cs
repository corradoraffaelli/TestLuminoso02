using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour {

	GameObject player;
	GameObject[] enemies;
	Collider2D[] playerColliders;
	Collider2D platformCollider;
	Collider2D[][] enemyColliders;

	Rigidbody2D rigidBody;
	SpriteRenderer spriteRenderer;
	float maxFallingSpeed;

	bool playerWasOver = false;
	bool playerOver = false;

	bool[] enemyWasOver;
	bool[] enemyOver;
	int[] enemyToIgnoreProgressive;

	float sogliaControllo = 0.0f;

	//public float maxSogliaControllo = 0.6f;
	//bool playerWasBelow = false;
	//bool needToIgnore = false;
	//bool needToNotIgnore = false;

	int toIgnoreProgressive = 0;
	public bool debugVariables = false;
	public int threshBeforeIgnore = 10;

	public bool useSpriteLimits = false;

	public float refreshEnemyTime = 2.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerColliders = player.GetComponents<Collider2D>();
		platformCollider = transform.GetComponent<Collider2D> ();
		rigidBody = player.GetComponent<Rigidbody2D>();
		maxFallingSpeed = player.GetComponent<PlayerMovements> ().maxFallingSpeed;

		//prima inizializzazione del collider
		colliderInit ();

		//ogni due secondi aggiorna gli array dei nemici e dei loro colliders
		InvokeRepeating("findEnemiesCollidersAndInit", 1, refreshEnemyTime);
	}

	void FixedUpdate()
	{
		setCollidersPlayerFU ();
		setCollidersEnemiesFU ();
	}

	//chiamata allo start, per inizializzare i colliders
	void colliderInit()
	{
		//player
		if (!useSpriteLimits) {
			playerOver = isPlayerOver (getPlayerBottomCollider (playerColliders).bounds, platformCollider.bounds);
		} else {
			spriteRenderer = player.GetComponent<SpriteRenderer> ();
			playerOver = isPlayerOver (getPlayerBottomCollider (playerColliders).bounds, spriteRenderer.bounds);
		}
		
		if (playerOver) {
			changeIgnoreCollider (playerColliders, false);
			playerWasOver = true;
			if (debugVariables)
				Debug.Log ("init settato");
		}
		
		if (!playerOver) {
			changeIgnoreCollider(playerColliders, true);
			playerWasOver = false;
			if (debugVariables)
				Debug.Log ("init ignorato");
		}

		//enemies
		findEnemiesCollidersAndInit ();
	}



	void setCollidersPlayerFU()
	{
		if (!useSpriteLimits) {
			playerOver = isPlayerOver (getPlayerBottomCollider (playerColliders).bounds, platformCollider.bounds);
		} else {
			spriteRenderer = player.GetComponent<SpriteRenderer> ();
			playerOver = isPlayerOver (getPlayerBottomCollider (playerColliders).bounds, spriteRenderer.bounds);
		}
		
		if (playerOver)
			toIgnoreProgressive = 0;
		//Debug.Log (playerOver);
		if (playerOver && !playerWasOver) {
			changeIgnoreCollider (playerColliders, false);
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
			changeIgnoreCollider(playerColliders, true);
			playerWasOver = false;
			toIgnoreProgressive = 0;
			if (debugVariables)
				Debug.Log ("da ignorare");
		}
	}

	void setCollidersEnemiesFU()
	{
		for (int i = 0; i<enemyColliders.Length; i++) {
			if (enemyColliders[i] != null)
			{
				Collider2D bottomCollider = getPlayerBottomCollider (enemyColliders[i]);
				if (bottomCollider != null)
				{
					enemyOver[i] = isPlayerOver (bottomCollider.bounds, platformCollider.bounds);
					
					if (enemyOver[i])
						enemyToIgnoreProgressive[i] = 0;
					
					if (enemyOver[i] && !enemyWasOver[i]) {
						changeIgnoreCollider (enemyColliders[i], false);
						enemyWasOver[i] = true;
						if (debugVariables)
							Debug.Log ("enemy da settare");
					}
					
					if (!enemyOver[i] && enemyWasOver[i] && enemyToIgnoreProgressive[i] < threshBeforeIgnore) {
						enemyToIgnoreProgressive[i] ++;
						if (debugVariables)
							Debug.Log (enemyToIgnoreProgressive[i]);
					}
					
					if (!enemyOver[i] && enemyWasOver[i] && enemyToIgnoreProgressive[i] == threshBeforeIgnore) {
						changeIgnoreCollider(enemyColliders[i], true);
						enemyWasOver[i] = false;
						enemyToIgnoreProgressive[i] = 0;
						if (debugVariables)
							Debug.Log (" enemy da ignorare");
					}
				}

			}

		}
	}

	//ritorna il collider con la y più bassa
	Collider2D getPlayerBottomCollider(Collider2D[] inputColliders)
	{
		bool primo = true;
		float limit = 0.0f;
		Collider2D basso= null;
		for (int i = 0; i< inputColliders.Length; i++) {
			if (inputColliders[i]!= null)
			{
				if (primo)
				{
					limit = inputColliders[i].bounds.min.y;
					basso = inputColliders[i];
					primo = true;
				}else{
					if (playerColliders[i].bounds.min.y < limit)
					{
						limit = inputColliders[i].bounds.min.y;
						basso = inputColliders[i];
					}
				}
			}

		}
		return basso;
	}

	bool isPlayerOver(Bounds playerCollider, Bounds platCollider)
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

		float playerLimit = playerCollider.min.y;
		float platLimit = platCollider.max.y;
		if (playerLimit > (platLimit-sogliaControllo))
			return true;
		else
			return false;
	}

	void changeIgnoreCollider(Collider2D[] inputColliders, bool ignoreOrNot = true)
	{
		for (int i = 0; i< inputColliders.Length; i++) {
			Physics2D.IgnoreCollision(inputColliders[i], platformCollider, ignoreOrNot);
		}
	}

	void findEnemiesCollidersAndInit()
	{
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		
		enemyColliders = new Collider2D[enemies.Length][];
		enemyOver = new bool[enemies.Length];
		enemyWasOver = new bool[enemies.Length];
		enemyToIgnoreProgressive = new int[enemies.Length];
		
		//scorro i nemici e riempio l'array di colliders
		for (int i = 0; i<enemies.Length; i++) {
			enemyColliders[i] = enemies[i].GetComponents<Collider2D>();

			enemyToIgnoreProgressive[i] = 0;
		}

		for (int i = 0; i<enemyColliders.Length; i++) {
			enemyOver[i] = isPlayerOver (getPlayerBottomCollider (enemyColliders[i]).bounds, platformCollider.bounds);
			
			if (enemyOver[i]) {
				changeIgnoreCollider (enemyColliders[i], false);
				enemyWasOver[i] = true;
				if (debugVariables)
					Debug.Log ("init enemy settato");
			}else{
				changeIgnoreCollider(enemyColliders[i], true);
				enemyWasOver[i] = false;
				if (debugVariables)
					Debug.Log ("init enemy ignorato");
			}
		}

	}
}
