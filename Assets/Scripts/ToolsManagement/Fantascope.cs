using UnityEngine;
using System.Collections;

public class Fantascope : Tool {
	
	//public GameObject lantern;
	
	public GameObject glasses;
	
	//INITIALIZATION PART------------------------------------------------------------------------------------------------------------------
	
	protected override void initializeTool() {
		
		//inventory = GameObject.Find ("Inventory");
		//getUsableGlasses ();
		
	}
	
	/*
	void getUsableGlasses(){
		
		foreach (Transform child in inventory.transform) {
			if(child.name=="Glasses") {
				glasses = child.gameObject;
				break;
			}
		}
		
		foreach (Transform child in glasses.transform) {
			if(child.tag=="basicGlass" || child.tag=="animatedGlass") {
				//glasses = child.gameObject;
				Glass newG = new Glass(child.GetComponent<SpriteRenderer>().sprite, child.tag);
				subTools.Add(newG);
			}
		}
		
	}
	*/
	
	//UPDATING PART-------------------------------------------------------------------------------------------------------------------------
	
	//qui va inserita la logica del tool, usata nell'update...
	protected override void useTool() {
		toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);
		Debug.Log ("sto usando il fantascopio");
	}
	
	
}
