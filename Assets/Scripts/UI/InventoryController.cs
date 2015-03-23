using UnityEngine;
using System.Collections;
using System;

public class InventoryController : MonoBehaviour {

	public bool useImage = true;
	public GameObject inventoryCanvas;
	public GameObject forziereGameObject;
	bool abilitato = false;

	// Use this for initialization
	void Start () {
		if (!useImage) {
			forziereGameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!useImage) {
			if (Input.GetMouseButtonDown(1))
				switchInventory();
		}
	}

	public void switchInventory()
	{
		float scala = (float)Convert.ToInt16 (abilitato);
		Time.timeScale = scala;
		abilitato = !abilitato;
		inventoryCanvas.SetActive (abilitato);
	}
}
