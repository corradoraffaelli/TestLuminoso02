using UnityEngine;
using System.Collections;

public class WindGlassModifier : MonoBehaviour {

	[System.Serializable]
	public class GlassState{
		public Sprite glassSprite;
		public float angle;
	}

	[SerializeField]
	public GlassState[] glassStates = new GlassState[4];

	public int actualStateIndex = 0;
	bool needToChangeAngle = false;

	GameObject magicLanternLogicObject;
	GlassesManager glassesManager;
	GlassesUIManager glassesUIManager;
	GraphicLantern graphicLantern;

	// Use this for initialization
	void Start () {
		magicLanternLogicObject = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		glassesManager = magicLanternLogicObject.GetComponent<GlassesManager> ();
		glassesUIManager = magicLanternLogicObject.GetComponent<GlassesUIManager> ();
		graphicLantern = magicLanternLogicObject.GetComponent<GraphicLantern> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void glassModifier()
	{
		//Debug.Log ("vetrino vento modificatore");
		setNextStateIndex ();
		GetComponent<Glass> ().changeGlassSprite (getActualState().glassSprite);
		glassesUIManager.updateUI ();
		needToChangeAngle = true;
	}

	GlassState getActualState()
	{
		return (glassStates [actualStateIndex]);
	}

	void setNextStateIndex()
	{
		if (actualStateIndex < (glassStates.Length - 1)) 
			actualStateIndex++;
		else
			actualStateIndex = 0;

	}

	void LateUpdate()
	{
		//if (needToChangeAngle) {
		//	needToChangeAngle = false;
		if (GetComponent<Glass>().actual && GetComponent<Glass>().canBeModified)
			graphicLantern.rotateTempProjectionByAngle (getActualState ().angle);
		//}
	}
}
