using System;
using UnityEngine;

public class Glass : MonoBehaviour
{
	// da impostare a private
	public bool usable = false;

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


}


