using UnityEngine;
using System.Collections;

public class SequenceButtonsDoor : MonoBehaviour {

	Transform myTrasform;

	public Transform targetPos;
	Vector3 defaultPos;

	[Range(0.1f,30.0f)]
	public float tForwardLenght = 10.0f;
	
	[Range(0.1f,30.0f)]
	public float tBackwardLenght = 20.0f;

	private float distanceToCover;
	private float forwardSpeed;
	private float backwardSpeed;

	int stepMax = 0;

	killWhatever kw;

	// Use this for initialization
	void Start () {
		myTrasform = transform;
		
		setDefaultPos ();
		
		checkTargetPos ();
		
		setDistanceToCover ();
		
		if(isKillableObj ())
			checkSpikes ();

	}

	#region STARTMETHODS

	private void setDefaultPos(){
		
		defaultPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); 
		
	}

	private void setDistanceToCover(){
		
		Vector3 dist = targetPos.position - myTrasform.position;
		
		distanceToCover = dist.magnitude;
		
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

	private bool isKillableObj(){
		
		foreach (Transform child in transform) {
			
			if (child.name == "Spikes") {
				
				kw = child.GetComponentInChildren<killWhatever> ();
				
			}
			
		}
		
		if (kw == null) {
			
			//Debug.Log ("ATTENZIONE - manca il crusher figlio della porta");
			return false;
		}
		else {
			
			return true;
		}
		
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

	#endregion STARTMETHODS



	// Update is called once per frame
	void Update () {



	}

	void applyMove(Vector3 target, bool isForward) {

		Vector3 dist = target - myTrasform.position;

		if (isForward) {
			forwardSpeed = distanceToCover / tForwardLenght;
		}
		else {
			backwardSpeed = distanceToCover / tBackwardLenght;
		}

		if (dist.magnitude < distanceToCover * (1.0f - ((stepMax) * 0.05)) ) {

			myTrasform.Translate (dist.normalized * (isForward ? forwardSpeed : backwardSpeed) * Time.deltaTime);
		
		}
	}

	public void setStepMax(int _step) {



		stepMax = _step;

	}

}
