using System;
using UnityEngine;

public class Glass : MonoBehaviour
{
	public bool usable = false;
	public bool canBeEnabled = true;

	public bool Usable {
		get{ 
			return usable;
		}
		set{
			usable = value;
		}
	}

	public Sprite spriteObject;
	public Sprite goodProjection;
	public Sprite badProjection;
	public Sprite blurredProjection;
	public Sprite emptySprite;
	public Sprite glassSprite;

	public string glassType;

	public GameObject[] subGlassList;

	//oggetto proiettato ddal vetrino, public perché deve essere usato dallo script della lanterna
	[HideInInspector]
	public GameObject projectionObject;


	//prfab dell'oggetto istanziato dal vetrino, va imposto manualmente
	public GameObject prefabObject;

	//variabili da settare se è un vetrino di fine livello
	public bool endingLevelGlass = false;
	public GameObject toOverlap;
	public int overlapPercentage = 10;
	[HideInInspector]
	public bool endedProjected = false;

	void Update(){
		 //if (usable == false && canBeEnabled)
		//	usable = controlSubGlassList ();
	}

	//ora inutilizzato
	bool controlSubGlassList()
	{
		foreach (GameObject subGlass in subGlassList) {
			subGlass subGlassScript = subGlass.GetComponent<subGlass>();
			if (subGlassScript.taken == false)
				return false;
		}
		canBeEnabled = false;
		return true;
	}


	public bool controlIfOverlap(Bounds inObjBOunds)
	{
		if (toOverlap) {
			BoxCollider2D BCOut = toOverlap.GetComponent<BoxCollider2D> ();

			float xpixels = BCOut.bounds.size.x * overlapPercentage / 100;
			float ypixels = BCOut.bounds.size.y * overlapPercentage / 100;



			if (inObjBOunds.min.x > (BCOut.bounds.min.x - xpixels) &&
				inObjBOunds.min.y > (BCOut.bounds.min.y - ypixels) &&
				inObjBOunds.max.x < (BCOut.bounds.max.x + xpixels) &&
				inObjBOunds.max.y < (BCOut.bounds.max.y + ypixels) &&

				inObjBOunds.min.x < (BCOut.bounds.min.x + xpixels) &&
				inObjBOunds.min.y < (BCOut.bounds.min.y + ypixels) &&
				inObjBOunds.max.x > (BCOut.bounds.max.x - xpixels) &&
				inObjBOunds.max.y > (BCOut.bounds.max.y - ypixels))
				return true;
			else
				return false;
		} else {
			return false;
		}
	}

	public void activeEndingLevelObjects()
	{
		foreach (Transform child in toOverlap.transform) {
			child.gameObject.SetActive(true);
		}
	}

}


