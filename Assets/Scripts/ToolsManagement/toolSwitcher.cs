using UnityEngine;
using System.Collections;

public class toolSwitcher : MonoBehaviour {


	public Tool[] toolList;
	public int activeToolIndex = 0;
	int actualActiveToolIndex = 0;

	public bool usingTool = true;
	bool actualUsingTool = false;
	
	//public GlassesUIManager glassesUIManager;

	//GameObject player;

	void Start () {
		//player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update () {
		//cambio il parametro di usabilità o meno dei tool o cambio tool quando il parametro di usabilità è attivo
		if ((actualUsingTool != usingTool) || ((actualActiveToolIndex != activeToolIndex) && usingTool))
		{
			if (usingTool)
			{
				for (int i = 0; i<activeToolIndex; i++)
					setActiveTool(i, false);
				setActiveTool (activeToolIndex, true);

				actualUsingTool = usingTool;
				actualActiveToolIndex = activeToolIndex;
			}
				
			else
			{
				for (int i = 0; i<activeToolIndex; i++)
					setActiveTool(i, false);
				actualUsingTool = usingTool;
			}
		}

	}

	//attiva la funzione per attivare il Tool, vedi classe Tool
	public void setActiveTool(int index, bool act){
		if (act == true) {
			if (!toolList[index].canBeActivated())
			{
				return;
			}
		}
		toolList [index].Active(act);
	}
	
	public void setNextUsableTool(){

		//setActiveTool (activeToolIndex, false);

		activeToolIndex++;

		activeToolIndex = activeToolIndex % toolList.Length;

		//setActiveTool (activeToolIndex, true);

	}

	//attiva/disattiva l'utilizzo dei tool
	/*
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
	*/

}
