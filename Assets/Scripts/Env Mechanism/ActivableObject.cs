using UnityEngine;
using System.Collections;

public class ActivableObject : MonoBehaviour {

	public bool DEBUG_transition = false;

	public enum buttonActivationType {
		OnePush,
		LongPush,
	}
	public buttonActivationType buttonType;

	public enum movementTyp {
		Translation,
		Rotation,
		RotoTranslation,
	}
	public movementTyp movementType;

	public enum endReactionType {
		ComeBack,
		Stay,
	}
	public endReactionType endActionType;

	public enum mechanismChainType {
		End,
		ContinueChain,

	}

	//public bool killWithSpikes = true;

	Vector3 defaultPos;
	public Transform targetPos;

	[Range(0.1f,30.0f)]
	public float tForwardLenght = 10.0f;

	[Range(0.1f,30.0f)]
	public float tBackwardLenght = 20.0f;

	bool forwardActionEnable = false;
	bool backwardActionEnable = false;

	private Transform myTrasform;


	private float distanceToCover;
	private float forwardSpeed;
	private float backwardSpeed;

	private bool freezePosition = false;
	private bool canReturn = true;

	killWhatever kw;

	// Use this for initialization
	void Start () {

		myTrasform = transform;

		setDefaultPos ();

		checkTargetPos ();

		//setSpeeds ();


		setDistanceToCover ();

		getKillWhateverScript ();

		checkSpikes ();
	}

	private void checkSpikes(){

		if (!kw.crusher) {

			bool found = false;

			foreach(Transform child in transform) {

				if(child.name == "Spikes"){

					SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

					if(sr != null) {
						found = true;
					}
					break;
				}

			}

			if(!found) {

				Debug.Log ("ATTENZIONE - spuntoni non trovati");

			}

		} 
		else {

			bool found = false;

			foreach(Transform child in transform) {
				
				if(child.name == "Spikes"){
					
					SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
					
					if(sr != null) {
						found = true;
					}
					break;
				}
				
			}

			if(!found) {
				
				Debug.Log ("ATTENZIONE - spuntoni presenti, sarebbe meglio rimuoverli!");
				
			}

		}

	}

	private void getKillWhateverScript(){

		foreach (Transform child in transform) {
			
			if (child.name == "Spikes") {

				kw = child.GetComponentInChildren<killWhatever> ();

			}

		}

		if (kw == null)
			Debug.Log ("ATTENZIONE - manca il crusher figlio della porta");

	}

	private void setDefaultPos(){

		defaultPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); 

	}

	private void setDistanceToCover(){

		Vector3 dist = targetPos.position - myTrasform.position;

		distanceToCover = dist.magnitude;

	}

	private void setSpeeds(){

		if(movementType== movementTyp.Translation) {
			
			Vector3 dist = targetPos.position - myTrasform.position;
			
			forwardSpeed = dist.magnitude / tForwardLenght;
			
			backwardSpeed = dist.magnitude / tBackwardLenght;
			
		}

	}

	private void checkTargetPos(){

		bool founded = false;

		if (targetPos == null) {

			foreach(Transform brother in transform.parent.transform) {

				if(brother.name=="TargetPos") {
					targetPos = brother.transform;
					founded = true;
					break;

				}

			}

			if (!founded) {
				Debug.Log ("ATTENZIONE - 'TargetPos' non trovato - controlla presenza oggetto e relativo nome, io sono : " + gameObject.name);
				
			}

		}



	}
	
	// Update is called once per frame
	void Update () {

		if (forwardActionEnable) {

			if(kw != null) {
				if(kw.crusher)
					kw.turnOn = false;

			}

			move (targetPos.position, true);

		}
		else {

			if(backwardActionEnable) {

				if(kw != null){
					if(kw.crusher)
						kw.turnOn = true;
				}

				move (defaultPos, false);

			}
			//fare qualcosa per tornare alla posizione di origine?

		}

	}
	

	private void translate(Vector3 target, bool isForward) {
		
		Vector3 dist = target - myTrasform.position;

		if (isForward) {
			forwardSpeed = distanceToCover / tForwardLenght;
		}
		else {
			backwardSpeed = distanceToCover / tBackwardLenght;
		}

		if (dist.magnitude > 0.1f) {
			if(!freezePosition)
				myTrasform.Translate (dist.normalized * (isForward ? forwardSpeed : backwardSpeed) * Time.deltaTime);

		}
		else {


			switch(endActionType) {

				case endReactionType.ComeBack : 
					if(DEBUG_transition) {
						Debug.Log ("arrivato a " + (isForward ? "target" : "inizio"));
					}
					if(isForward) {
						if(canReturn) {
							forwardActionEnable = false;
							backwardActionEnable = true;
						}
					}
					else {
						backwardActionEnable = false;
					}

					break;

				case endReactionType.Stay :
					if(DEBUG_transition) {
						Debug.Log ("arrivato a " + (isForward ? "target" : "inizio") + ", sto fermo");
					}
					break;

				default :
					break;

			}




		}
		//myTrasform.Translate(
		
	}

	private void rotate() {
		
		
	}

	private void move(Vector3 target, bool isForward) {

		switch (movementType) {

			case movementTyp.Translation :
				translate(target, isForward);
				break;

			case movementTyp.Rotation :
				rotate();
				break;

			default :
				
				break;

		}

	}
	


	public void buttonPushed(bool bp){
		
		switch (endActionType) {
			
		case endReactionType.ComeBack :

			freezePosition = false;

			switch(buttonType) {
				
				case buttonActivationType.OnePush :
					if(bp) {
						//attivazione bottone
						forwardActionEnable = true;
						backwardActionEnable = false;
						canReturn = false;

					}
					else {
						canReturn = true;
					}
					break;
				case buttonActivationType.LongPush :
					if(bp) {
						//attivazione bottone
						forwardActionEnable = true;
						backwardActionEnable = false;
						canReturn = false;
					}
					else {
						//caso non di interesse per ora...
						forwardActionEnable = false;
						backwardActionEnable = true;
						canReturn = true;
					}
					break;
				default :
					break;
			}
			
			break;

		case endReactionType.Stay :

			switch(buttonType) {
				
				case buttonActivationType.OnePush :
					if(bp) {
						//attivazione bottone
						forwardActionEnable = true;
						backwardActionEnable = false;
					}
					else {
						//non faccio niente...
					}
					break;
				case buttonActivationType.LongPush :
					if(bp) {
						//attivazione bottone
						forwardActionEnable = true;
						backwardActionEnable = false;
						freezePosition = false;
					}
					else {
						//caso non di interesse per ora...
						forwardActionEnable = false;
						backwardActionEnable = true;
						freezePosition = true;
					}
					break;
				default :
					break;
			}

			break;
			
		default :
			break;
			
		}
		
		
	}

}
