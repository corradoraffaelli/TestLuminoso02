using UnityEngine;
using System.Collections;

public class ChainActivationObjStarter : MonoBehaviour {
	
	public GameObject firstChainPieceObj;
	private ChainActivationObjPiece firstChainPieceScript;
	public string []triggerTagList;
	public bool NeedInteractionButton = false;
	int realTriggerTagListLength;

	public Sprite buttonUp;
	public Sprite buttonDown;

	SpriteRenderer sr;

	private GameObject whoIsActivatingMe;
	bool processInCourse = false;
	
	// Use this for initialization
	void Start () {

		sr = GetComponent<SpriteRenderer> ();

		checkTagList ();
		if(checkObjectToActivate ())
			getFirstChainPieceScript ();
	}

	void Update() {

		if(processInCourse && whoIsActivatingMe!=null){
			if (!whoIsActivatingMe.activeInHierarchy) {
				//OnTriggerEnter2D (c);
				//Debug.Log ("DISATTIVOOOOOOOOOOOOOOOOOOOOOOOO");
				if(firstChainPieceScript!=null) {
					//Debug.Log ("A!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					//firstChainPieceObj.SendMessage("buttonPushed", false);
					firstChainPieceScript.c_buttonPushed(false);
					whoIsActivatingMe = null;
				}
				return;
			}
			else {
				//Debug.Log ("ATTIVOOOOOOOOOOOOOOOOOOOOOOOO");

			}
		}

	}
	
	private void checkTagList(){
		
		foreach (string s in triggerTagList) {
			
			if(s=="")
				Debug.Log ("ATTENZIONE - UN TAG È VUOTO");
			
		}
		
		if (triggerTagList.Length == 0) {
			
			Debug.Log ("ATTENZIONE - UN TAG È VUOTO");
		}
	}
	
	private bool checkObjectToActivate(){
		
		if (firstChainPieceObj == null) {
			Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
			return false;
		}
		return true;
	}

	private void getFirstChainPieceScript() {

		firstChainPieceScript = firstChainPieceObj.GetComponent<ChainActivationObjPiece> ();
		if(firstChainPieceScript==null)
			Debug.Log ("ATTENZIONE - NON trovato lo script chainactivationobjpiece");
	}

	//TODO: implementare controllo su chi fa push, tengo variabile conservata e confronto all'exit
	public void OnTriggerEnter2D(Collider2D c) {

		//Debug.Log ("entro");

		if (NeedInteractionButton)
			return;
		
		if (whoIsActivatingMe != null)
			return;
		
		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(firstChainPieceScript!=null) {
					whoIsActivatingMe = c.gameObject;
					processInCourse = true;
					//firstChainPieceObj.SendMessage("buttonPushed", true);
					sr.sprite = buttonDown;
					firstChainPieceScript.c_buttonPushed(true);
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}
		
		
	}
	
	public void OnTriggerExit2D(Collider2D c) {

		//Debug.Log ("esco");

		if (NeedInteractionButton)
			return;
		
		if (whoIsActivatingMe != c.gameObject)
			return;
		
		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(firstChainPieceScript!=null) {
					//firstChainPieceObj.SendMessage("buttonPushed", false);
					firstChainPieceScript.c_buttonPushed(false);
					sr.sprite = buttonUp;
					whoIsActivatingMe = null;
					processInCourse = false;
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}
		
		
	}
	
	
	public void OnTriggerStay2D(Collider2D c) {

		for (int i=0; i< triggerTagList.Length; i++) {

			if(c.tag== triggerTagList[i]) {

				/*
				if (whoIsActivatingMe == null) {
					//OnTriggerEnter2D (c);
					if(firstChainPiece!=null) {
						Debug.Log ("A!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
						firstChainPiece.SendMessage("buttonPushed", false);
						whoIsActivatingMe = null;
						processInCourse = false;
					}
					return;
				}
				*/
				if (!NeedInteractionButton)
					return;
				
				if (!Input.GetKeyUp (KeyCode.Return))
					return;

						
				if(firstChainPieceScript!=null) {
					//firstChainPieceObj.SendMessage("buttonPushed", true);
					firstChainPieceScript.c_buttonPushed(true);
					sr.sprite = buttonDown;
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
					
			}
			
		}
		
		
	}

	public void c_manualActivation() {

		//firstChainPieceObj.SendMessage("buttonPushed", true);
		if(firstChainPieceScript!=null)
			firstChainPieceScript.c_buttonPushed(true);
		else {
			Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
		}
	}
	
	
}
