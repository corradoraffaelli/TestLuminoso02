using UnityEngine;
using System.Collections;

public class WindBehaviour : MonoBehaviour {

	GameObject[] collidingObjects = new GameObject[20];

	GameObject magicLanternLogicObject;
	GlassesManager glassesManager;
	//WindGlassModifier windGlassModifier
	//GlassesUIManager glassesUIManager;
	GraphicLantern graphicLantern;
	
	// Use this for initialization
	void Start () {
		magicLanternLogicObject = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		glassesManager = magicLanternLogicObject.GetComponent<GlassesManager> ();
		//glassesUIManager = magicLanternLogicObject.GetComponent<GlassesUIManager> ();
		graphicLantern = magicLanternLogicObject.GetComponent<GraphicLantern> ();
	}

	void Update()
	{
		if (graphicLantern != null) {
			GetComponent<AreaEffector2D>().forceDirection = graphicLantern.getStandardFakeProjectionRotation();
			//GetComponent<AreaEffector2D>().forceDirection = 120;
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
