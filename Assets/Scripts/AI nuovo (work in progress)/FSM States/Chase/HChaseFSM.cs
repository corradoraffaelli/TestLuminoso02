using UnityEngine;
using System.Collections;

public class HChaseFSM : HEnemyStateFSM {
	
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

	
	//CONSTRUCTOR----------------------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------


	public HChaseFSM(int _stateId, GameObject _gameo, int _hLevel, HEnemyStateFSM _fatherState, AIAgent1 _scriptAIAgent)  :
		base("Chase", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		
		myInitialize += initializeChase;
		
		myUpdate += updateChase;
		
		myFinalize += finalizeChase;

		initializeChaseParameters ();
	}

	public void setDefaultTransitions(HStunnedFSM stunState, HPatrolFSM patrolState) {

		//addTransition (C2ScheckStunned, "Stunned");

		addTransition (C2ScheckStunned, stunState);
		//addTransition (C2PlostTarget, "Patrol");
		addTransition (C2PlostTarget, patrolState);

		//addTransition( stun)

	}

	#region INITIALIZECHASEPARAMETERS

	void initializeChaseParameters(){
		
		chasePar = gameObject.GetComponent<AIParameters> ().chaseParameters;
		
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
	

	
	#endregion INITIALIZECHASEPARAMETERS

	#region MYINITIALIZE

	private void initializeChase(){
		/*
		if (ob != null) {
			Debug.Log ("mi passano : " + ((GameObject) ob).name);
			chaseTarget = ((GameObject) ob);
			Debug.Log ("assegnato il target a chasetarget : " + chaseTarget.name );
		}
		else {
			Debug.Log (" CHASE NULL ");
		}
		*/
		Debug.Log ("inizio CHASE -----------------DA NON USAREE!!!!!!!!!!!!!!!!!!!!");

		IEnumerator ciaone = inizioChase ();

		Debug.Log ("ciaone è " + ciaone);

		//StartCoroutine (ciaone);
		_StartCoroutine (inizioChase ());
		//agentScript.StartCoroutine (inizioChase ());

	}

	public IEnumerator inizioChase() {

		Debug.Log ("ciao 1 CHASE --------------------------------");

		yield return null;

		Debug.Log ("ciao 2 CHASE --------------------------------");



	}

	#endregion MYINITIALIZE

	#region MYUPDATE
	
	private void updateChase(){
		Debug.Log ("update da CHASE");
		moveTowardTarget (chaseTarget, chaseSpeed);
	}

	#endregion MYUPDATE

	#region MYFINALIZE
	
	private void finalizeChase(){
		Debug.Log ("finisco CHASE --------------------------------");
		
		//return null;
	}

	#endregion MYFINALIZE

	#region MYTRANSITIONS
	
	private bool C2PlostTarget(){
		
		//target -> NULL
		if (chaseTarget == null) {

			#if _TRANSITION_DEBUG
				Debug.Log ("CHASE 2 PATROL - target null");
			#endif	
			
			return true;
		}
		
		
		//target -> different height
		if (Mathf.Abs (chaseTarget.transform.position.y - transform.position.y) > 2.0f) {
			#if _TRANSITION_DEBUG
				Debug.Log ("CHASE 2 PATROL - target different height");
			#endif			
			
			return true;
			
		}
		
		
		//target -> too distant
		if (Vector3.Distance (chaseTarget.transform.position, transform.position) > RangeOfView + 2.0f) {

			#if _TRANSITION_DEBUG
				Debug.Log ("CHASE 2 PATROL - target too far");
			#endif

			return true;
			
		}

		return false;
		
	}

	private bool C2ScheckStunned(){
		
		if (par.stunnedReceived) {
			
			par.stunnedReceived = false;
			return true;
		} 
		else {
			return false;
		}
	}
	
	#endregion MYTRANSITIONS
	
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
