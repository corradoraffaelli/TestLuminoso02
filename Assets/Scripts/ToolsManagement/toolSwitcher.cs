using UnityEngine;
using System.Collections;

public class toolSwitcher : MonoBehaviour {


	public Tool[] toolList;
	public int activeToolIndex;

	public bool usingTool = false;

	public bool useTapOnPlayer = false;

	
	
	public GlassesUIManager glassesUIManager;

	GameObject player;

	void Start () {
		//setActiveTool (activeToolIndex, true);
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update () {
		/*
		if (Input.GetMouseButtonDown (1) && !Input.GetMouseButtonDown (0) && !Input.GetMouseButtonDown (2)) {
			//tasto destro mouse ma non contemporaneo a sx e centrale
			switchTool ();

		}
		*/


		//utilizzo un tool se clicco ul personaggio, o premo left_shift, solo se esiste almeno un oggetto nella lista
		/*
		if (toolList.GetLength(0) != 0.0) {
			if (Input.GetButtonUp ("Fire1") && !useTapOnPlayer) {
				usingTool = !usingTool;
				useTool (usingTool);
			}

			if (Input.GetMouseButtonDown (0) && useTapOnPlayer) {
				Vector2 pos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
				RaycastHit2D hitInfo = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (pos), Vector2.zero);
				if (hitInfo) {
					if (hitInfo.transform.gameObject.tag == "Player") {
						usingTool = !usingTool;
						useTool (usingTool);
					}
				}
			}
		}
		*/


		//attivo la lanterna se tengo premuto il rispettivo tasto, non è attiva ed il player è a terra
		//
		if (Input.GetButton ("Mira") && !usingTool && player.GetComponent<PlayerMovements>().OnGround) {
			usingTool = true;
			useTool (usingTool);
		}

		if (Input.GetMouseButtonUp(0))
		{

			useTool (false);
			switchUsingTool (false);
		}

		/*
		if (Input.GetKeyUp (KeyCode.E)) {
			Debug.Log ("dentro");
			//switch del vetrino
			MagicLantern ML = toolList[activeToolIndex] as MagicLantern;

			ML.c_nextGlass();

			//ML.c_showActualGLass();
			if (glassesUIManager!= null)
			{
				glassesUIManager.setGlassSize();
			}
		}
		*/

		/*
		if (Input.GetButtonDown ("Mira")) {
			usingTool = !usingTool;
			useTool (usingTool);
		}
		*/
	}

	//attiva la funzione per attivare il Tool, vedi classe Tool
	void setActiveTool(int index, bool act){
		if (act == true) {
			if (!toolList[index].canBeActivated())
			{
				usingTool = false;
				return;
			}
				
		}
		toolList [index].Active(act);
	}
	
	void switchTool(){

		setActiveTool (activeToolIndex, false);

		activeToolIndex++;

		activeToolIndex = activeToolIndex % toolList.Length;

		setActiveTool (activeToolIndex, true);

	}

	//attiva/disattiva l'utilizzo dei tool
	public void useTool(bool useOrNot)
	{
		setActiveTool(activeToolIndex, useOrNot);
	}

	//seleziona il tool da usare
	public void chooseTool (int toolIndex)
	{
		setActiveTool(activeToolIndex, false);
		activeToolIndex = toolIndex;
	}

	public void switchUsingTool(bool UseOrNot)
	{
		usingTool = UseOrNot;
	}

}
