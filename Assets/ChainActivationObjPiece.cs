using UnityEngine;
using System.Collections;

public class ChainActivationObjPiece : MonoBehaviour {
	
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
	
	public enum ChainPieceType {
		LastPiece,
		NotLastPiece,
	}
	public ChainPieceType chainPieceType;

	public enum nextPieceActivationType {
		Immediately,
		AtEndPosition,
	}
	public nextPieceActivationType nextActivationTime;
	


	//public bool killWithSpikes = true;
	
	Vector3 defaultPos;
	public Transform targetPos;
	public float targetDegree = 360.0f;
	private float startDegree;

	public GameObject nextChainPiece;
	private bool nextChainPieceStarted = false;
	
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

		getDefaultAngle ();

		setDefaultPos ();
		
		checkTargetPos ();

		setDistanceToCover ();
		
		if(isKillableObj ())
			checkSpikes ();
	}

	private void getDefaultAngle() {

		startDegree = transform.rotation.z;

	}

	private void checkSpikes(){
		
		if (!kw.crusher) {
			
			bool found = false;
			
			foreach(Transform child in transform) {
				
				if(child.tag == "KillingObj"){
					
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
				
				if(child.tag == "KillingObj"){
					
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
	
	private bool isKillableObj(){
		
		foreach (Transform child in transform) {
			
			if (child.name == "Spikes") {
				
				kw = child.GetComponentInChildren<killWhatever> ();
				
			}
			
		}
		
		if (kw == null) {

			Debug.Log ("ATTENZIONE - manca il crusher figlio della porta");
			return false;
		}
		else {

			return true;
		}
		
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

	//--------------------------------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	// Update is called once per frame
	void Update () {
		
		if (forwardActionEnable) {
			
			if(kw != null) {
				if(kw.crusher)
					kw.turnOn = false;
				
			}

			applyMove(targetPos.position, true);
			
		}
		else {
			
			if(backwardActionEnable) {
				
				if(kw != null){
					if(kw.crusher)
						kw.turnOn = true;
				}
				
				applyMove(defaultPos, false);
				
			}
			//fare qualcosa per tornare alla posizione di origine?
			
		}
		
	}

	private bool checkArriveCondition(Vector3 target, bool isForward) {

		Vector3 dist;

		switch (movementType) {
			
		case movementTyp.Translation:

			dist = target - myTrasform.position;
			if(dist.magnitude < 0.2f) {
				Debug.Log ("arrived translation");
				return true;
			}
			break;
			
		case movementTyp.Rotation:

			if(isForward) {
				if(transform.localRotation.eulerAngles.z >= (targetDegree - 0.5f)) {
					Debug.Log ("arrived targetpos rotation");
					return true;
				}

			}
			else {
				if(transform.localRotation.eulerAngles.z < startDegree + 0.5f) {
					Debug.Log ("arrived startpos rotation");
					return true;
				}

			}



			break;

		case movementTyp.RotoTranslation:

			dist = target - myTrasform.position;
			if(transform.localRotation.eulerAngles.z >= (targetDegree - 5.0f) && dist.magnitude < 0.2f){
				Debug.Log ("arrived rototranslation");
				return true;
			}

			break;

		default :
			
			break;

		}

		return false;

	}
	
	private void applyMove(Vector3 target, bool isForward) {

		bool arriveCondition = false;

		arriveCondition = checkArriveCondition (target, isForward);

		if (!arriveCondition) {


			handleMovementSituation(target, isForward);

		}
		else {
			
			handleEndSituation(target, isForward);

		}

	}

	private void handleMovementSituation(Vector3 target, bool isForward) {

		Vector3 dist = target - myTrasform.position;

		if(!freezePosition) {
			
			switch(movementType) {
				
			case movementTyp.Translation :
				if (isForward) {
					forwardSpeed = distanceToCover / tForwardLenght;
				}
				else {
					backwardSpeed = distanceToCover / tBackwardLenght;
				}
				Debug.Log ("translo");
				myTrasform.Translate (dist.normalized * (isForward ? forwardSpeed : backwardSpeed) * Time.deltaTime);
				
				break;
				
			case movementTyp.Rotation :
				if (isForward) {
					forwardSpeed = targetDegree / tForwardLenght;
				}
				else {
					backwardSpeed = targetDegree / tBackwardLenght;
				}
				Debug.Log ("ruoto");
				myTrasform.Rotate(Vector3.forward, (isForward ? forwardSpeed : -backwardSpeed) * Time.deltaTime);
				
				break;
				
				
			case movementTyp.RotoTranslation :
				if (isForward) {
					forwardSpeed = distanceToCover / tForwardLenght;
				}
				else {
					backwardSpeed = distanceToCover / tBackwardLenght;
				}
				
				myTrasform.Translate (dist.normalized * (isForward ? forwardSpeed : backwardSpeed) * Time.deltaTime);
				
				if (isForward) {
					forwardSpeed = targetDegree / tForwardLenght;
				}
				else {
					backwardSpeed = targetDegree / tBackwardLenght;
				}
				
				myTrasform.Rotate(Vector3.forward, (isForward ? forwardSpeed : -backwardSpeed) * Time.deltaTime);
				
				Debug.Log ("rototraslo");
				
				break;
			default :
				
				break;
				
			}
			
			
			
		}
		
		if(!isForward && chainPieceType == ChainPieceType.NotLastPiece && nextChainPiece!=null && nextChainPieceStarted) {
			
			Debug.Log ("il meccanismo " + gameObject.name + " inizia a tornare indietro e avvisa quello dopo");
			nextChainPiece.SendMessage("buttonPushed", false);
			nextChainPieceStarted = false;
			
		}
		
		switch (nextActivationTime) {
		case nextPieceActivationType.AtEndPosition :
			
			break;
			
		case nextPieceActivationType.Immediately :
			if(nextChainPiece!=null) {
				
				if(!nextChainPieceStarted && isForward) {
					Debug.Log ("il meccanismo " + gameObject.name + " è arrivato alla fine e avvisa il mecc dopo");
					nextChainPiece.SendMessage("buttonPushed", true);
					nextChainPieceStarted = true;
				}
			}
			break;
			
		default :
			break;
		}

	}

	private void handleEndSituation(Vector3 target, bool isForward) {

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
		
		
		
		switch(chainPieceType) {
			
		case ChainPieceType.NotLastPiece :
			
			switch (nextActivationTime) {
			case nextPieceActivationType.AtEndPosition :
				if(nextChainPiece!=null) {
					
					if(!nextChainPieceStarted && isForward) {
						Debug.Log ("il meccanismo " + gameObject.name + " è arrivato alla fine e avvisa il mecc dopo");
						nextChainPiece.SendMessage("buttonPushed", true);
						nextChainPieceStarted = true;
					}
				}
				break;
				
			case nextPieceActivationType.Immediately :
				break;
				
			default :
				break;
			}
			break;
			
		case ChainPieceType.LastPiece :
			
			
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
