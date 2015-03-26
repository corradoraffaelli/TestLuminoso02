using System;
using UnityEngine;

public class Glass : MonoBehaviour
{
	// da impostare a private
	public bool usable = false;
	public bool attractor = false;
	public bool canInstantiateObj = false;
	public GameObject toOverlap;

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
	public string glassType;

	public GameObject[] subGlassList;

	//oggetto proiettato ddal vetrino, public perché deve essere usato dallo script della lanterna
	public GameObject projectionObject;

	void Update(){
		 if (usable == false)
			usable = controlSubGlassList ();
	}

	bool controlSubGlassList()
	{
		foreach (GameObject subGlass in subGlassList) {
			subGlass subGlassScript = subGlass.GetComponent<subGlass>();
			if (subGlassScript.taken == false)
				return false;
		}
		return true;
	}


	public bool controlIfOverlap(Bounds inObjBOunds)
	{
		BoxCollider2D BCOut = toOverlap.GetComponent<BoxCollider2D> ();
		if (inObjBOunds.min.x > BCOut.bounds.min.x &&
		    inObjBOunds.min.y > BCOut.bounds.min.y &&
		    inObjBOunds.max.x < BCOut.bounds.max.x &&
		    inObjBOunds.max.y < BCOut.bounds.max.y)
		    return true;
		else
		    return false;

	}


}


