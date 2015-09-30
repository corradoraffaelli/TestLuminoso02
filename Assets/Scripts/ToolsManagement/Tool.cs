/*
 * metodi da estendere :
 * 
 * initializeTool()
 * 
 * useTool()
 * 
 * 
 * 
*/

using UnityEngine;
using System.Collections;

abstract public class Tool : MonoBehaviour {

	//dice se è il tool momentaneamente in uso del giocatore
	//cioè, ad esempio se sto perlomeno mirando
	public bool active;
	public bool activable;

	//il tool può essere bloccante per il controllo del giocatore o meno
	public bool freezingTool;

	//riferimento al player
	protected GameObject player;

	//possibile riferimento ad un gameobject che rappresenta il mio tool
	protected GameObject toolGameObject;

	protected GameObject controller;
	protected CursorHandler cursorHandler;

	protected InputKeeper inputKeeper;
	//true se sto riattivando il tool, quindi è true se fino a quel momento è stato
	//inattivo, sarà false lungo tutto il suo utilizzo, tornerà true a fine utilizzo
	bool activation = true;

	//---------------------------------
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		controller = GameObject.FindGameObjectWithTag ("Controller");
		cursorHandler = controller.GetComponent<CursorHandler> ();
		inputKeeper = controller.GetComponent<InputKeeper> ();
		initializeTool ();
	}


	// Update is called once per frame
	void Update () {

		if (!PlayStatusTracker.inPlay) {
			useToolPaused();
			return;
		}
			

		if (!active) {
			return;
		} 
		else {
			if(activation) {
				activation = false;
				if(freezingTool)
					freezeController(true);
				activationToolFunc();
			}
			useTool ();
		}
	}


	//VIRTUAL FUNCTIONS--------------------------------------------------------------------------------------------------------------------------

	//richiamata nello start per inizializzare il tool
	protected virtual void initializeTool() {
		//inventory = GameObject.Find ("Inventory");
		
	}

	//funzione invocata ogni volta che si usa il tool, serve per bloccare il controller
	protected void freezeController(bool freez){
		//GameObject.FindGameObjectWithTag("Player").SendMessage("isUsingTool", freez);
		player.SendMessage("isUsingTool", freez);
	}

	//funzione per usare il tool, quindi per esempio per proiettare il vetrino, oppure per sparare col fucile
	abstract protected void useTool();

	//funzione per usare il tool, ma quando la partita non è in play
	abstract protected void useToolPaused();

	//funzione da poter richiamare alla riattivazione del tool
	protected virtual void activationToolFunc () {

	}

	//funzione da poter richiamare alla disattivazione del tool
	protected virtual void disactivationToolFunc () {

	}


	//TOOL SWITCHER INTERFACE----------------------------------------------------------------------------------------------------------------------

	public void Active(bool act) {

		//se lavoro con un gameobject esterno, lo attivo/disattivo
		//la lanterna non lo usa per ora, fa in altro modo
		if(toolGameObject!=null)
			toolGameObject.SetActive (act);

		if (act) {
			active = true;
		} 
		else {
			active = false;
			//se l'oggetto dovesse venir cambiato mentre lo uso
			freezeController(false);
			activation = true;
			disactivationToolFunc();
		}
	}
	
	public virtual bool canBeActivated(){
		return true;
	}
}
