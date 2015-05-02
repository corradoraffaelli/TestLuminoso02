using UnityEngine;
using System.Collections;

public class ActivableObject : MonoBehaviour {


	public enum buttonActivationType {
		OnePush,
		StayPush,
	}
	public buttonActivationType buttonType;

	public enum movementTyp {
		Translation,
		Rotation,
	}
	public movementTyp movementType;

	public enum endReactionType {
		ComeBack,
		Stay,
	}
	public endReactionType endActionType;
	
	Vector3 defaultPos;
	public Transform targetPos;

	[Range(0.1f,30.0f)]
	public float tForwardLenght = 10.0f;

	[Range(0.1f,30.0f)]
	public float tBackwardLenght = 20.0f;

	bool forwardActionEnable = false;
	bool backwardActionEnable = false;

	private Transform myTrasform;

	private float forwardSpeed;
	private float backwardSpeed;

	killWhatever kw;

	// Use this for initialization
	void Start () {

		myTrasform = transform;

		setDefaultPos ();

		checkTargetPos ();

		setSpeeds ();

		getKillWhateverScript ();

	}

	private void getKillWhateverScript(){

		kw = GetComponentInChildren<killWhatever> ();

		if (kw == null)
			Debug.Log ("ATTENZIONE - manca il crusher figlio della porta");

	}

	private void setDefaultPos(){

		defaultPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); 

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

			kw.turnOn = false;
			move (targetPos.position, true);

		}
		else {

			if(backwardActionEnable) {

				kw.turnOn = true;
				move (defaultPos, false);

			}
			//fare qualcosa per tornare alla posizione di origine?

		}

	}

	/*
	private void translate() {

		Vector3 dist = targetPos.position - myTrasform.position;

		if (dist.magnitude > 0.5f)
			myTrasform.Translate (dist.normalized * forwardSpeed);
		else {
			Debug.Log ("arrivato");
			backwardActionEnable = true;
		}
		//myTrasform.Translate(

	}
	*/

	private void translate(Vector3 target, bool isForward) {
		
		Vector3 dist = target - myTrasform.position;
		
		if (dist.magnitude > 0.1f)
			myTrasform.Translate (dist.normalized * (isForward ? forwardSpeed : backwardSpeed) * Time.deltaTime );
		else {


			switch(endActionType) {

				case endReactionType.ComeBack : 
					Debug.Log ("arrivato a " + (isForward ? "target" : "inizio"));
					if(isForward) {
						forwardActionEnable = false;
						backwardActionEnable = true;
					}
					else {
						backwardActionEnable = false;
					}

					break;

				case endReactionType.Stay :
					Debug.Log ("arrivato a " + (isForward ? "target" : "inizio") + ", sto fermo");
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
			switch(buttonType) {
				
				case buttonActivationType.OnePush :
					if(bp) {
						forwardActionEnable = bp;
						backwardActionEnable = !bp;
					}
					break;
				case buttonActivationType.StayPush :
					Debug.Log ("Questa configurazione non ha molto senso");
					break;
				default :
					break;
			}

			break;

		case endReactionType.Stay :

			switch(buttonType) {

				case buttonActivationType.OnePush :
					forwardActionEnable = !forwardActionEnable;
					backwardActionEnable = !forwardActionEnable;
					break;
				case buttonActivationType.StayPush :
					forwardActionEnable = bp;
					backwardActionEnable = !bp;
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
