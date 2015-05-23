using UnityEngine;
using System.Collections;

public class GlassesManager : MonoBehaviour {

	public string modifierMethodName = "glassModifier";
	public Glass[] glassList;
	
	Glass[] usableGlassList;

	int actualGlassIndex = 0;
	int actualGlassIndexUsableList = 0;

	GameObject controller;
	InputKeeper inputKeeper;
	//public Glass actualGlass;

	void Awake(){
		updateUsableGlassList ();
		nextUsableGlass (true);
	}

	void Start () {
		controller = GameObject.FindGameObjectWithTag ("Controller");
		if (controller != null)
			inputKeeper = controller.GetComponent<InputKeeper> ();
		//prima inizializzazione della lista di vetrini usabili

	}

	void Update () {
		//DEBUG----------
		if (inputKeeper!=null && inputKeeper.isButtonUp("NextGlass")) {
			nextUsableGlass ();
		}
		/*
		if (Input.GetKeyUp (KeyCode.M)) {
			enableGlass (2, true);
		}
		if (Input.GetKeyUp (KeyCode.N)) {
			enableGlass (3, false);
		}
		*/
		//se sono raccolti tutti i frammenti di un vetrino, si attiva il vetrino
		if (isThereAGlass ())
		{
			for (int i = 0; i<glassList.Length; i++) {
				if (glassList[i].Usable == false && glassList[i].canBeEnabled)
				{

					if (controlSubGlassList(glassList[i]))
					    enableGlass (i, true);
				}
			}
		}
		//updateUsableGlassList ();
		updateActualGlassBool ();

	}

	void updateActualGlassBool()
	{
		for (int i = 0; i<glassList.Length; i++) {
			if (glassList[i] != null)
				glassList[i].actual = false;
		}

		glassList [actualGlassIndex].actual = true;
	}

	public void callGlassModifier()
	{
		getActualGlass ().SendMessage (modifierMethodName, SendMessageOptions.DontRequireReceiver);
	}

	public bool isGlassModifiable()
	{
		return getActualGlass ().canBeModified;
	}

	//controllo dei frammenti del vetrino passato come input
	bool controlSubGlassList(Glass glass)
	{
		foreach (GameObject subGlass in glass.subGlassList) {
			if (subGlass)
			{
				subGlass subGlassScript = subGlass.GetComponent<subGlass>();
				if (subGlassScript.taken == false)
					return false;
			}

		}
		glass.canBeEnabled = false;
		return true;
	}

	//ritorna vero se c'è almeno un vetrino
	//falso se la lista è vuota, oppure è stata settata una dimensione dall'inspector, ma non sono inclusi elementi
	public bool isThereAGlass()
	{
		if (glassList.Length == 0)
			return false;
		else 
		{
			for (int i = 0; i<glassList.Length ; i++)
			{
				if (glassList[i] != null)
					return true;
			}
			return false;
		}
	}

	//ritorna vero se c'è almeno un vetrino usabile
	//falso se la lista è vuota, oppure è stata settata una dimensione dall'inspector, ma non sono inclusi elementi
	public bool isThereAUsableGlass()
	{
		if (glassList.Length == 0)
			return false;
		else 
		{
			for (int i = 0; i<glassList.Length ; i++)
			{
				if (glassList[i].Usable == true)
					return true;
			}
			return false;
		}
	}

	//ritorna il prossimo vetrino usabile e aggiorna l'actualGlassIndex
	//se non c'è un altro vetrino usabile oltre quello attuale ritorna di nuovo quello attuale
	Glass nextUsableGlass(bool first = false)
	{
		if (isThereAUsableGlass ()) {
			//salvo l'indice del vetrino prima dell'incremento
			int indexBeforeIncrement = actualGlassIndex;

			while (true) {
				if (!first)
				{
					if (actualGlassIndex < (glassList.Length - 1)) {
						actualGlassIndex = actualGlassIndex + 1;
					} else {
						actualGlassIndex = 0;
					}
				}

				if (glassList [actualGlassIndex].Usable) {
					updateActualGlassIndexUsableList();
					return glassList [actualGlassIndex];
				} else {
					if (actualGlassIndex == indexBeforeIncrement && first == false) {
						return null;
					}
				}	
				first = false;
			}
		} else {
			return null;
		}
	}

	public Glass getActualGlass()
	{
		return glassList [actualGlassIndex];
	}

	public Glass[] getGlassList()
	{
		return glassList;
	}
	
	//aggiorna la lista dei vetrini usabili
	//da richiamare ogni volta che si aggiorna l'usabilità o meno di un vetrino

	public void updateUsableGlassList()
	{
		int glassesNum = 0;
		if (isThereAGlass()) {
			//calcolo il numero di vetrini usabili
			for (int i = 0; i<glassList.Length ; i++)
			{
				if (glassList[i].Usable)
					glassesNum++;
			}

			usableGlassList = new Glass[glassesNum];
			int usableIndex = 0;

			for (int i = 0; i<glassList.Length ; i++)
			{
				if (glassList[i].Usable)
				{
					usableGlassList[usableIndex] = glassList[i];
					usableIndex++;
				}
					
			}
		}else{
			usableGlassList = new Glass[0];
		}
	}

	public Glass[] getUsableGlassList()
	{
		return usableGlassList;
	}

	public int getActualGlassIndex()
	{
		return actualGlassIndex;
	}

	void updateActualGlassIndexUsableList()
	{
		string name = glassList [actualGlassIndex].glassType;

		int usableIndex;
		for (int i = 0; i < usableGlassList.Length; i++) {
			if (usableGlassList[i].glassType == name)
			{
				actualGlassIndexUsableList = i;
			}
		}
	}

	public int getActualGlassIndexUsableList()
	{
		return actualGlassIndexUsableList;
	}

	public void enableGlass(int glassIndexToEnable, bool enable = true)
	{
		Debug.Log ("abilitato glass " + glassIndexToEnable);

		if (enable) {
			glassList[glassIndexToEnable].usable = true;
			actualGlassIndex = glassIndexToEnable;
		}else{
			glassList[glassIndexToEnable].usable = false;
			if (actualGlassIndex == glassIndexToEnable)
			{
				nextUsableGlass();
				//actualGlassIndex = actualGlassIndex;
			}
		}

		//ogni volta che si aggiorna la lista di vetrini, bisogna riaggiornare anche quella di quelli usabili
		updateUsableGlassList ();
		updateActualGlassIndexUsableList();
	}

	public void enableGlassByName(string glassName, bool enable = true)
	{


		if (enable) {
			for (int i = 0; i<glassList.Length; i++)
			{
				if (glassList[i] != null && glassList[i].glassType == glassName)
				{
					glassList[i].usable = true;
					actualGlassIndex = i;
					Debug.Log ("abilitato glass " + glassName);
				}
			}
		}else{
			for (int i = 0; i<glassList.Length; i++)
			{
				if (glassList[i] != null && glassList[i].glassType == glassName)
				{
					glassList[i].usable = false;
					if (actualGlassIndex == i)
					{
						nextUsableGlass();
						Debug.Log ("Disabilitato glass " + glassName);
						//actualGlassIndex = actualGlassIndex;
					}
				}
			}
		}
		
		//ogni volta che si aggiorna la lista di vetrini, bisogna riaggiornare anche quella di quelli usabili
		updateUsableGlassList ();
		updateActualGlassIndexUsableList();
	}

}
