using UnityEngine;
using System.Collections;

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
	}

	void Update()
	{
		if (graphicLantern != null) {
			GetComponent<AreaEffector2D>().forceDirection = graphicLantern.getStandardFakeProjectionRotation();
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
}
