using UnityEngine;
using System.Collections;

public class ChaseFSM : StateFSM {

	ChaseParameters chasePar;

	GameObject chaseTarget {
		get{ 
			if(chasePar!=null) return chasePar.chaseTarget;
			else return null;}
		set{ if(chasePar!=null) chasePar.chaseTarget = value;}
		
	}
	
	float chaseSpeed {
		get{ 
			if(chasePar!=null) return chasePar.chaseSpeed;
			else return 0.0f;}
		set{ if(chasePar!=null) chasePar.chaseSpeed = value;}
		
	}

	float RangeOfView {
		get{ 
			if(chasePar!=null) return chasePar.RangeOfView;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.RangeOfView = value;}
		
	}

	float AdditionalROV {
		get{ 
			if(chasePar!=null) return chasePar.AdditionalROV;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.AdditionalROV = value;}
		
	}

	//CONSTRUCTOR----------------------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	#region constructor

	public ChaseFSM(GameObject gameo) : base(gameo) {

		//GameObject gameo = this.gameObject;

		state = myStateName.Chase;

		myInitialize -= initializeState;
		myInitialize += initializeChase;

		myUpdate -= updateState;
		myUpdate += updateChase;

		myFinalize -= finalizeState;
		myFinalize += finalizeChase;

		myTransitions = new myStateTransition[1];

		myTransitions[0] += C2PlostTarget;
		//myTransitions[1] += chaseToFleeTransition;

		initializeChaseParameters ();
	}

	void initializeChaseParameters(){

		chasePar = myGameObject.GetComponent<AIParameters> ().chaseParameters;
		
		if (chasePar != null) {
			if(!getRangeOfView()) {
				//arbitrario valore di default 
				RangeOfView = 3.0f;
			}
		} 
		else {
			Debug.Log ("ATTENZIONE - PatrolParameters non trovato perché non è stato trovato AIParameters");
		}
		
	}

	bool getRangeOfView(){
		
		GameObject range = null;
		
		foreach (Transform child in transform) {
			if(child.gameObject.name=="RangeOfView") {
				range = child.gameObject;
			}
			
		}
		
		if (range != null) {
			
			if(range.transform.localPosition.x < 0) 
				Debug.Log("ATTENZIONE - L'empty 'RangeOfView' è in una posizione negativa");
			
			RangeOfView = Mathf.Abs( range.transform.localPosition.x ) * Mathf.Abs( transform.localScale.x );
			
			return true;
			
		}
		else {
			
			return false;
			
		}
		
	}

	void setAdditionalROVHalfOfROV(){

		if(RangeOfView != 0.0f)
			AdditionalROV = RangeOfView / 2.0f;

	}

	#endregion constructor

	private void initializeChase(ref object ob){

		if (ob != null) {
			Debug.Log ("mi passano : " + ((GameObject) ob).name);
			chaseTarget = ((GameObject) ob);
			Debug.Log ("assegnato il target a chasetarget : " + chaseTarget.name );
		}
		else {
			Debug.Log (" CHASE NULL ");
		}
		Debug.Log ("inizio CHASE --------------------------------");
	}
	
	private void updateChase(){
		Debug.Log ("ciao da CHASE");
	}
	
	private object finalizeChase(){
		Debug.Log ("finisco CHASE --------------------------------");

		return null;
	}

	private void C2PlostTarget(ref myStateName st){

		//target -> NULL
		if (chaseTarget == null) {

			if(par.DEBUG_FSM_TRANSITION[0])
				Debug.Log ("CHASE 2 PATROL - target null");

			st = myStateName.Patrol;
			return;
		}


		//target -> different height
		if (Mathf.Abs (chaseTarget.transform.position.y - transform.position.y) > 2.0f) {

			if(par.DEBUG_FSM_TRANSITION[0])
				Debug.Log ("CHASE 2 PATROL - target different height");

			st = myStateName.Patrol;
			return;
			
		}


		//target -> too distant
		if (Vector3.Distance (chaseTarget.transform.position, transform.position) > RangeOfView + AdditionalROV) {

			if(par.DEBUG_FSM_TRANSITION[0])
				Debug.Log ("CHASE 2 PATROL - target too far");

			st = myStateName.Patrol;
			return;

		}

	}



	/*
	private void C2FcheckDanger(ref myStateName st){
		
		if (st != state)
			return;
		
		RaycastHit2D hit;
		
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 0.8f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 0.8f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 0.8f), Color.yellow);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.8f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, fleeLayer);
		
		if (hit.collider != null) {
			
			st = myStateName.Flee;
			_fleeTarget = hit.transform.gameObject;
			return;
		}
		
	}
	*/
}
