using UnityEngine;
using System.Collections;

public class TriggerSequenceButton : MonoBehaviour {

	public int sequenceNumber;
	public bool pushed = false;
	public bool untouchable = false;

	//[HideInInspector]
	public Sprite pushedImg;
	//[HideInInspector]
	public Sprite notPushedImg;

	SpriteRenderer sr;

	public GameObject handler;
	GameObject actualPusher;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if ( (c.tag == "Player" || c.tag == "Enemy") && c.gameObject != actualPusher && !untouchable) {

			if(!pushed) {
				//Debug.Log ("inizio trigg da " + c.name);
				pushed = true;
				actualPusher = c.gameObject;
				handler.SendMessage("c_sequenceButtonPressed", sequenceNumber);
				//Debug.Log ("fine trigg da " + c.name);
			}
		}

	}

	public void OnTriggerExit2D(Collider2D c) {

		if(actualPusher == c.gameObject)
			actualPusher = null;

	}

	public IEnumerator animateGoToTargetPos() {

		pushed = true;

		yield return new WaitForSeconds(0.1f);

		//suono click

		sr.sprite = pushedImg;


	}

	public IEnumerator animateReturnDefaultPos() {

		untouchable = true;

		yield return new WaitForSeconds(0.5f);

		//suono clack

		sr.sprite = notPushedImg;

		pushed = false;
		untouchable = false;
	}
}
