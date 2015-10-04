using UnityEngine;
using System.Collections;

/// <summary>
/// Classe responsabile del bottone. (DEPRECATA)
/// </summary>

//Dario

public class ButtonTrigger : MonoBehaviour {

	public GameObject objectToActivate;
	public string []triggerTagList;
	public bool NeedInteractionButton = false;
	int realTriggerTagListLength;

	private GameObject whoIsActivatingMe;

	// Use this for initialization
	void Start () {

		checkTagList ();
		checkObjectToActivate ();

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

		if(objectToActivate==null)
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

				if(objectToActivate!=null) {
					whoIsActivatingMe = c.gameObject;
					objectToActivate.SendMessage("buttonPushed", true);
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
				
				if(objectToActivate!=null) {
					objectToActivate.SendMessage("buttonPushed", false);
					whoIsActivatingMe = null;
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}


	}


	public void OnTriggerStay2D(Collider2D c) {

		if (whoIsActivatingMe == null) {
			OnTriggerEnter2D (c);
			return;
		}

		if (!NeedInteractionButton)
			return;

		if (!Input.GetKeyUp (KeyCode.Return))
			return;


		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(objectToActivate!=null) {
					objectToActivate.SendMessage("buttonPushed", true);
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}
		
		
	}


}
