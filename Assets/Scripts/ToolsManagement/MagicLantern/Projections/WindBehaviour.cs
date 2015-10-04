using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il comportamento della proiezione del vento.
/// </summary>

// Corrado
public class WindBehaviour : MonoBehaviour {

	GameObject[] collidingObjects = new GameObject[20];

	GameObject magicLanternLogicObject;
	GlassesManager glassesManager;
	//WindGlassModifier windGlassModifier
	//GlassesUIManager glassesUIManager;
	GraphicLantern graphicLantern;

	AreaEffector2D areaEffector;
	GameObject player;
	PlayerMovements playerMovements;
	bool activeOnPlayer = true;

	public float adaptingSpeed = 100f;

	GameObject cerchio;
	ParticleSystem partSystem;

	// Use this for initialization
	void Start () {
		magicLanternLogicObject = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		glassesManager = magicLanternLogicObject.GetComponent<GlassesManager> ();
		//glassesUIManager = magicLanternLogicObject.GetComponent<GlassesUIManager> ();
		graphicLantern = magicLanternLogicObject.GetComponent<GraphicLantern> ();
		areaEffector = GetComponent<AreaEffector2D> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null)
			playerMovements = player.GetComponent<PlayerMovements> ();

		getCircleAndParticles();
		setPartSystemLife();
	}

	void Update()
	{
		if (graphicLantern != null) {
			GetComponent<AreaEffector2D>().forceAngle = graphicLantern.getStandardFakeProjectionRotation();
			//GetComponent<AreaEffector2D>().forceDirection = 120;
		}

		if (playerMovements.onLadder && activeOnPlayer) {
			activeOnPlayer = false;

			// Bit shift dell'indice del player per avere una layer mask con solo il bit del player attivo
			int layerMask = 1 << (LayerMask.NameToLayer ("Player"));
			
			//L'operatore ~ inverte una layerMask (perciò si ottiene la layerMask con tutto, escluso il player
			layerMask = ~layerMask;

			//cambio quindi la layermask su cui ha effetto il vento
			areaEffector.colliderMask = layerMask;
		}

		if (!playerMovements.onLadder && !activeOnPlayer) {
			//la layer mask -1 indica tutti i layer
			areaEffector.colliderMask = -1;

			activeOnPlayer = true;
		}
			
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		collidingObjects [firstEmptyIndex()] = other.gameObject;

		Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
		if (rigidbody != null && !rigidbody.isKinematic)
		{
			//if (rigidbody.velocity.y < 0.0f)
				//rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y/3);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		for (int i = 0; i<collidingObjects.Length; i++) {
			if (collidingObjects[i] == other.gameObject)
				collidingObjects[i] = null;
		}
	}

	int firstEmptyIndex()
	{
		//int tempIndex = 0;
		for (int i = 0; i<collidingObjects.Length; i++) {
			if (collidingObjects[i] == null)
				return i;
		}
		return 0;
	}

	public bool isGameObjectOnWind(GameObject inputGameObject)
	{
		for (int i = 0; i<collidingObjects.Length; i++) {
			if (collidingObjects[i] == inputGameObject)
				return true;
		}

		return false;
	}

	void LateUpdate()
	{
		/*
		for (int i = 0; i<collidingObjects.Length; i++) {

			if (collidingObjects[i] != null)
			{
				Rigidbody2D rigidbody = collidingObjects[i].GetComponent<Rigidbody2D>();
				if (rigidbody != null && !rigidbody.isKinematic)
				{
					//controllo anche se è onground
					PlayerMovements movements = collidingObjects[i].GetComponent<PlayerMovements>();
					//procedo solo se l'oggetto non ha lo script playerMovements o, nel caso ce l'abbia, che non sia onGround
					if (movements == null || (movements != null && !movements.onGround))
					{
						//deve adattare gli angoli
						Debug.Log (collidingObjects[i].gameObject.name);

						/*
						if (rigidbody.velocity.y < 0.0f)
							rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y/3);
						*/
						//rigidbody.velocity = vectorAdaptation(rigidbody.velocity);
		/*
					}
					
				}
			}
		}
		*/
	}

	void OnTriggerStay2D(Collider2D other)
	{
		/*
		Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
		if (rigidbody != null && !rigidbody.isKinematic)
		{
			//controllo anche se è onground
			PlayerMovements movements = other.GetComponent<PlayerMovements>();
			//procedo solo se l'oggetto non ha lo script playerMovements o, nel caso ce l'abbia, che non sia onGround
			if (movements == null || (movements != null && !movements.onGround))
			{
				//deve adattare gli angoli
				Debug.Log (other.gameObject.name);

				rigidbody.velocity = vectorAdaptation(rigidbody.velocity);
			}

		}
		*/
	
	}

	Vector2 vectorAdaptation(Vector2 inputVector)
	{
		//prendo la direzione del vento come "right" del gameObject
		Vector3 objDirectionVec3 = transform.right;
		Vector2 objDirectionVec2 = new Vector2 (objDirectionVec3.x, objDirectionVec3.y);

		float directionMagn = objDirectionVec2.magnitude;
		float velocityMagn = inputVector.magnitude;

		float newX = velocityMagn * objDirectionVec2.x / directionMagn;
		float newY = velocityMagn * objDirectionVec2.y / directionMagn;

		Vector2 objDirection = new Vector2(newX, newY);

		Vector2 newVector = Vector2.Lerp(inputVector, objDirection, adaptingSpeed*Time.deltaTime*100);
		return (newVector);
	}

	void getCircleAndParticles()
	{
		if (transform.parent != null && transform.parent.transform.gameObject!= null)
		{
			if (transform.parent.transform.gameObject.name == "raggio_cerchio")
				cerchio = transform.parent.transform.gameObject;
		}
		ParticleSystem tempPartSystem = GetComponentInChildren<ParticleSystem>();
		if (tempPartSystem != null)
			partSystem = tempPartSystem;
	}

	void setPartSystemLife()
	{
		if (cerchio != null && partSystem != null)
		{
			float scale = cerchio.transform.localScale.x;
			partSystem.startLifetime = Mathf.Abs (scale/10.0f);
		}
	}
}
