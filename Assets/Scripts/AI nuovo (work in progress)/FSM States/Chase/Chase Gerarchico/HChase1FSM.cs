using UnityEngine;
using System.Collections;

public class HChase1FSM : HEnemyStateFSM {

	#region VARIABLES

	HChargeChaseFSM hChargeChase;
	HCrashChaseFSM hCrashChase;

	bool lostTarget = false;

	protected SpriteRenderer statusSpriteRend;

	protected ChaseParameters chasePar;
	
	protected GameObject chaseTarget {
		get{ 
			if(chasePar!=null) return chasePar.chaseTarget;
			else return null;}
		set{ if(chasePar!=null) chasePar.chaseTarget = value;}
		
	}

	protected float chaseSpeed {
		get{ 
			if(chasePar!=null) return chasePar.chaseSpeed;
			else return 0.0f;}
		set{ if(chasePar!=null) chasePar.chaseSpeed = value;}
		
	}
	
	protected float RangeOfView {
		get{ 
			if(chasePar!=null) return chasePar.RangeOfView;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.RangeOfView = value;}
		
	}
	
	protected float AdditionalROV {
		get{ 
			if(chasePar!=null) return chasePar.AdditionalROVBeforeLost;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.AdditionalROVBeforeLost = value;}
		
	}

	protected Sprite alarmedSprite {
		get{
			if (statusPar != null) {
				if (statusPar.alarmed != null)
					return statusPar.alarmed;
				else
					return null;
			}
			else
				return null;
		}

	}

	protected Sprite confusedSprite {
		get{
			if (statusPar != null) {
				if (statusPar.confused != null)
					return statusPar.confused;
				else
					return null;
			}
			else
				return null;
		}
		
	}

	#endregion VARIABLES

	public HChase1FSM(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
	: base(_stateName, 0, _gameo, _hLevel, false, null, _scriptAIAgent) {
		
		finalHLevel = false;

		initializeChaseParameters ();

		myFinalize += finalizeChase;

		if (!getStatusSpriteRenderer (ref statusSpriteRend))
			Debug.Log ("ATTENZIONE - spriterenderer di StatusImg non trovato");
		//myHandleCollisionEnter
	}

	#region INITIALIZECHASEPARAMETERS
	

	protected void initializeChaseParameters(){
		
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
	
	void setAdditionalROVHalfOfROV(){
		
		if(RangeOfView != 0.0f)
			AdditionalROV = RangeOfView / 2.0f;
		
	}
	
	#endregion INITIALIZECHASEPARAMETERS

	public void setDefaultInitialize() {

		myInitialize += initChaseFather;

	}

	public void setDefaultStates(HChargeChaseFSM hchargec, HCrashChaseFSM hcrashc) {

		hChargeChase = hchargec;
		hCrashChase = hcrashc;

		addState (hChargeChase);
		addState (hCrashChase);

	}

	public void setDefaultCollision() {

		myHandleCollisionEnter += checkKillPlayerCollision;

	}

	public void setDefaultTransitions(HStunnedFSM hstun, HPatrolFSM hpatrol) {

		addTransition (C2ScheckStun, hstun);
		addTransition (C2PlostTarget, hpatrol);
		addTransition (Any2KScheckInstantKill, hstun);

	}

	public void setDefaultTransitions(HStunnedFSM hstun, HPatrol1FSM hpatrol) {
		
		addTransition (C2ScheckStun, hstun);
		addTransition (C2PlostTarget, hpatrol);
		
	}

	protected void initChaseFather() {

		activeState = hChargeChase;
		/*
		if (ob != null) {

			#if _DEBUG
				Debug.Log ("mi passano : " + ((GameObject) ob).name);
			#endif

			chaseTarget = ((GameObject) ob);

		}
		else {
			Debug.Log (" ATTENZIONE - CHASE NULL ");
		}
		*/

		BasicMessageFSM [] mess = takeFinalizeMessages<BasicMessageFSM> ();

		if (mess != null) {

			chaseTarget = mess[0].getTarget();

		}



	}



	bool C2ScheckStun() {
		
		if (par.stunnedReceived) {
			
			par.stunnedReceived = false;
			
			return true;
		} 
		else {
			return false;
		}
		
	}

	bool C2PlostTarget(){


		bool lost = false;

		//target -> NULL
		if (chaseTarget == null) {
			
			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target null");
			#endif
			
			lost = true;
		}
		
		
		//target -> different height
		if (Mathf.Abs (chaseTarget.transform.position.y - transform.position.y) > 2.0f) {

			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target different height");
			#endif

			lost = true;
			
		}
		
		
		//target -> too distant
		if (Vector3.Distance (chaseTarget.transform.position, transform.position) > RangeOfView + AdditionalROV) {
			
			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target too far");
			#endif
			
			lost = true;
			
		}

		if (lost) {

			lostTarget = true;

		}

		return lost;
		
	}

	protected void finalizeChase() {
		
		//object ob = null;

		if (lostTarget) {

			BasicMessageFSM pame = new BasicMessageFSM("Suspicious");
			//ob = (object) pame;

			//Debug.Log("aggiunto messaggio");
			addFinalizeMessage(pame);

		}

		//return ob;

	}

}







public class HChargeChaseFSM : HChase1FSM {

	IEnumerator chargeCor;
	bool charged = false;

	public HChargeChaseFSM(GameObject _gameo, int _hLevel, HEnemyStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("ChargeChase", _gameo, _hLevel, _scriptAIAgent) {

		finalHLevel = true;
		fatherState = _fatherState;
		myInitialize += initializeChargeChase;
		myFinalize += finalizeChargeChase;

	}

	public void setDefaultTransitions(HCrashChaseFSM hcrash) {

		addTransition (CC2CCcheckFinishCharge, hcrash);

	}

	protected void initializeChargeChase() {

		#if _DEBUG
			Debug.Log ("init da " + stateName + " ----------------------------------------");
		#endif


		charged = false;

		chargeCor = countDownCharge (1.0f);

		statusSpriteRend.sprite = alarmedSprite;

		_StartCoroutine (chargeCor);

	}

	IEnumerator countDownCharge(float _seconds) {

		yield return new WaitForSeconds(_seconds);
		charged = true;

	}

	void chargeUpdate() {

		//Debug.Log ("update charge");

	}

	bool CC2CCcheckFinishCharge() {

		if(charged) {
			//_StopCoroutine(chargeCor);
			return true;
		} 
		else {
			return false;
		}
	}

	protected void finalizeChargeChase() {

		//object ob = null;

		#if _DEBUG
			Debug.Log ("fine da " + stateName);
		#endif

		_StopCoroutine(chargeCor);
		
		charged = false;

		statusSpriteRend.sprite = null;

		//return ob;
	}

}

public class HCrashChaseFSM : HChase1FSM {
	
	IEnumerator chargeCor;
	bool wallCollision = false;
	public HCrashChaseFSM(GameObject _gameo, int _hLevel, HEnemyStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("CrashChase", _gameo, _hLevel, _scriptAIAgent) {
		
		finalHLevel = true;
		fatherState = _fatherState;
		myInitialize += initializeCrashChase;

		myUpdate += updateCrashChase;

		myFinalize += finalizeCrashChase;

		myHandleCollisionEnter += checkCrashWallCollision;

		
	}

	public void setDefaultTransitions(HStunnedFSM hstunn) {
		
		addTransition (CC2ScheckWallStun, hstunn);
		
	}

	protected void initializeCrashChase() {

		#if _DEBUG
		Debug.Log ("init da " + stateName);
		#endif
		
	}

	protected void updateCrashChase() {

		//Debug.Log ("udpate CRASSSHHHH");
		i_move (chaseSpeed);
		//moveTowardTarget (chaseTarget, chaseSpeed);


	}

	protected void finalizeCrashChase() {

		//object ob = null;

		#if _DEBUG
		Debug.Log ("fine da " + stateName);
		#endif

		//return ob;

		
	}

	bool CC2ScheckWallStun() {

		if (wallCollision) {

			wallCollision = false;
			
			return true;
		} 
		else {
			return false;
		}
		
	}



	protected void checkCrashWallCollision(Collision2D co) {

		if (co.gameObject.tag != "Player") {

			wallCollision = true;
			wallCrashRepercussion(co);

		}
		else {

		}
	}

	private void wallCrashRepercussion(Collision2D co) {

		int verseRepercussion = 1;

		ContactPoint2D []contactPoints =  co.contacts;

		
		foreach (ContactPoint2D cp in contactPoints) {

			if( (cp.point.x - transform.position.x ) > 0.0f)
				verseRepercussion = -1;
			else
				verseRepercussion = 1;
		}

		_rigidbody.velocity = new Vector2 (0.0f, 0.0f);
		_rigidbody.AddForce(new Vector2(verseRepercussion*150.0f, 0.0f));
		
	}



}
