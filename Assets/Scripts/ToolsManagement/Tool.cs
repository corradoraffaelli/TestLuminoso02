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

	//il tool può essere bloccante per il controllo del giocatore o meno
	public bool freezingTool;

	//prevedo una possibile interazione con un gameobject inventario
	public GameObject inventory;

	//lista di sotto oggetti usati dal tool, per esempio vetrini nel caso 
	protected ArrayList subTools;
	protected int subToolActiveIndex;

	//riferimento al player
	public GameObject player;

	//possibile riferimento ad un gameobject che rappresenta il mio tool
	public GameObject toolGameObject;

	public enum toolType {
		oneClick,
		twoClick,
		dragClick,
	}

	//tipo di tool che voglio implementare, in base al tipo di input
	public toolType tType;

	protected bool usingOneClick = false;
	protected bool[] usingTwoClick = {false, true};
	protected bool usingDrag = false;

	Vector3 clickMousePosition;
	protected Vector3 actualMousePosition;

	//true se sto riattivando il tool, quindi è true se fino a quel momento è stato
	//inattivo, sarà false lungo tutto il suo utilizzo, tornerà true a fine utilizzo
	bool activation = true;

	//---------------------------------
	
	// Use this for initialization
	void Start () {
		initializeTool ();
	}


	// Update is called once per frame
	void Update () {

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

			setActualMousePosition ();

			switch (tType) {

			case toolType.oneClick:
				if (Input.GetMouseButtonUp (0)) {
					//è responsabilità del tool esteso come e quando riportare la variabile a false
					usingOneClick = true;
					setClickMousePosition ();
				}

				break;

			case toolType.twoClick:
				if (Input.GetMouseButtonUp (0)) {

					if (!usingTwoClick [0]) {

						usingTwoClick [0] = true;
						usingTwoClick [1] = false;
						setClickMousePosition ();
					} else {
						if (!usingTwoClick [1]) {

							usingTwoClick [0] = false;
							usingTwoClick [1] = true;
							setClickMousePosition ();
						}

					}

				}

				break;

			case toolType.dragClick:
				if (Input.GetMouseButton (0)) {
					setClickMousePosition ();
					usingDrag = true;

				} 
				else {

					usingDrag = false;

				}

				break;

			default :
				break;
			}

			useTool ();

		}
	}

	//BASIC FUNCTIONS--------------------------------------------------------------------------------------------------------------------------

	//funzione per usare il prossimo subTool, per esempio il prossimo vetrino
	void switchSubTool(){
		
		subToolActiveIndex++;
		
		subToolActiveIndex = subToolActiveIndex % subTools.Count;

	}

	
	void setClickMousePosition() {
		clickMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 1.0f));
	}
	
	void setActualMousePosition() {
		actualMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 1.0f));
	}
	
	void followMouse() {
		
		transform.position = actualMousePosition;
		
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

	//funzione da poter richiamare alla riattivazione del tool
	protected virtual void activationToolFunc () {

	}

	//funzione da poter richiamare alla disattivazione del tool
	protected virtual void disactivationToolFunc () {

	}


	//TOOL SWITCHER INTERFACE----------------------------------------------------------------------------------------------------------------------

	public void Active(bool act) {

		//se lavoro con un gameobject esterno, lo attivo/disattivo
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

}
