using UnityEngine;
using System.Collections;

public class ChainActivationObjStarter : MonoBehaviour {
	
	public GameObject firstChainPiece;
	public string []triggerTagList;
	public bool NeedInteractionButton = false;
	int realTriggerTagListLength;
	
	private GameObject whoIsActivatingMe;
	bool processInCourse = false;
	
	// Use this for initialization
	void Start () {
		
		checkTagList ();
		checkObjectToActivate ();
		
	}

	void Update() {

		if(processInCourse && whoIsActivatingMe!=null){
			if (!whoIsActivatingMe.activeInHierarchy) {
				//OnTriggerEnter2D (c);
				//Debug.Log ("DISATTIVOOOOOOOOOOOOOOOOOOOOOOOO");
				if(firstChainPiece!=null) {
					Debug.Log ("A!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					firstChainPiece.SendMessage("buttonPushed", false);
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
	
	private void checkObjectToActivate(){
		
		if(firstChainPiece==null)
			Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
		
	}
	
	//TODO: implementare controllo su chi fa push, tengo variabile conservata e confronto all'exit
	public void OnTriggerEnter2D(Collider2D c) {
		
		if (NeedInteractionButton)
			return;
		
		if (whoIsActivatingMe != null)
			return;
		
		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(firstChainPiece!=null) {
					whoIsActivatingMe = c.gameObject;
					processInCourse = true;
					firstChainPiece.SendMessage("buttonPushed", true);
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}
		
		
	}
	
	public void OnTriggerExit2D(Collider2D c) {
		
		if (NeedInteractionButton)
			return;
		
		if (whoIsActivatingMe != c.gameObject)
			return;
		
		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(firstChainPiece!=null) {
					firstChainPiece.SendMessage("buttonPushed", false);
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

						
				if(firstChainPiece!=null) {
					firstChainPiece.SendMessage("buttonPushed", true);
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
					
			}
			
		}
		
		
	}

	public void c_manualActivation() {

		firstChainPiece.SendMessage("buttonPushed", true);

	}
	
	
}
