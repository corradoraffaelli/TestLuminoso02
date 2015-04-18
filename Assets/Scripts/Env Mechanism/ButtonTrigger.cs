using UnityEngine;
using System.Collections;

public class ButtonTrigger : MonoBehaviour {

	public GameObject objectToActivate;
	public string []triggerTagList;
	int realTriggerTagListLength;

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
	}

	private void checkObjectToActivate(){

		if(objectToActivate==null)
			Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");

	}

	//TODO: implementare controllo su chi fa push, tengo variabile conservata e confronto all'exit
	public void OnTriggerEnter2D(Collider2D c) {

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

	public void OnTriggerExit2D(Collider2D c) {

		for (int i=0; i< triggerTagList.Length; i++) {
			
			if(c.tag== triggerTagList[i]) {
				
				if(objectToActivate!=null) {
					objectToActivate.SendMessage("buttonPushed", false);
				}
				else {
					Debug.Log ("ATTENZIONE - L'OGGETTO DA ATTIVARE NON E' STATO ASSEGNATO");
				}
				break;
				
			}
			
		}


	}


}
