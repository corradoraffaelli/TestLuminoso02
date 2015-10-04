using UnityEngine;
using System.Collections;

/// <summary>
/// Stato intermedio Chase per la macchina a stati gerarchica.
/// </summary>

//Dario


public class HChase1FSM : HEnemyStateFSM {

	#region VARIABLES

	HChargeChaseFSM hChargeChase;
	HCrashChaseFSM hCrashChase;

	protected bool lostTarget = false;

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


	protected float timeToLoseTarget {
		get{ 
			if(chasePar!=null) return chasePar.timeToLoseTarget;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.timeToLoseTarget = value;}
		
	}
	
	protected float timeToCharge {
		get{ 
			if(chasePar!=null) return chasePar.timeToCharge;
			else return 0.0f;}
		set{ 
			if(chasePar!=null) chasePar.timeToCharge = value;}
		
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
		addTransition (C2PlostTargetNull, hpatrol);
		addTransition (Any2KScheckInstantKill, hstun);

	}

	public void setDefaultTransitions(HStunnedFSM hstun, HPatrol1FSM hpatrol) {
		
		addTransition (C2ScheckStun, hstun);
		addTransition (C2PlostTargetNull, hpatrol);
		addTransition (Any2KScheckInstantKill, hstun);
		
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

	bool C2PlostTargetNull(){


		bool lost = false;

		//target -> NULL
		if (chaseTarget == null) {
			
			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target null");
			#endif
			
			lost = true;
		}
		
		
		//target -> different height
		/*
		if (Mathf.Abs (chaseTarget.transform.position.y - transform.position.y) > 2.5f) {

			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target different height");
			#endif

			lost = true;
			
		}
		*/
		
		//target -> too distant
		/*
		if (Vector2.Distance (chaseTarget.transform.position, transform.position) > RangeOfView + AdditionalROV) {
			
			#if _DEBUG
				Debug.Log ("CHASE 2 PATROL - target too far");
			#endif
			
			lost = true;
			
		}
		*/

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

		weakPointCollider.offset = new Vector2 (0.0f, weakPointCollider.offset.y);
		weakPointCollider.size = new Vector2 (0.75f, weakPointCollider.size.y);

		//return ob;

	}

}


/// <summary>
/// Stato "foglia" Chase per la macchina a stati gerarchica. Derivato da HChase1FSM.
/// </summary>

//Dario

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

		chargeCor = countDownCharge (timeToCharge);

		statusSpriteRend.sprite = alarmedSprite;

		i_alert (true);

		_StartCoroutine (chargeCor);

	}

	IEnumerator countDownCharge(float _seconds) {

		yield return new WaitForSeconds((_seconds * 2.0f )/3.0f);

		//weakPointCollider.offset = new Vector2 (0.7f, weakPointCollider.offset.y);
		weakPointCollider.offset = new Vector2 (0.52f, weakPointCollider.offset.y);
		weakPointCollider.size = new Vector2 (1.1f, weakPointCollider.size.y);

		yield return new WaitForSeconds((_seconds * 1.0f )/3.0f);

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

		i_alert (false);

		statusSpriteRend.sprite = null;


		//return ob;
	}

}

/// <summary>
/// Stato "foglia" Chase per la macchina a stati gerarchica. Derivato da HChase1FSM.
/// </summary>

//Dario

public class HCrashChaseFSM : HChase1FSM {

	bool losingInFront = false;
	bool lostInFront = false;
	IEnumerator lostCor;
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

	public void setDefaultTransitions(HStunnedFSM hstunn, HPatrol1FSM hpatrol) {
		
		addTransition (CC2ScheckWallStun, hstunn);

		addTransition (CC2PcheckNotInFrontTarget, hpatrol);
		
	}

	protected void initializeCrashChase() {

		#if _DEBUG
		Debug.Log ("init da " + stateName);
		#endif
		//Debug.Log ("init da " + stateName);
		i_charged (true);

		//weakPointCollider.offset = new Vector2 (0.7f, weakPointCollider.offset.y);
		weakPointCollider.offset = new Vector2 (0.52f, weakPointCollider.offset.y);
		weakPointCollider.size = new Vector2 (1.1f, weakPointCollider.size.y);

	}

	protected void updateCrashChase() {

		//Debug.Log ("udpate CRASSSHHHH" + Time.deltaTime);
		i_move (chaseSpeed);

		audioHandler.playClipByName ("Morso");

		if (isTargetNotInFrontAnymore () && !losingInFront) {
			
			lostCor = countDownLostInFront (timeToLoseTarget);
			
			_StartCoroutine(lostCor);
			
			losingInFront = true;

		}
		//moveTowardTarget (chaseTarget, chaseSpeed);


	}

	protected void finalizeCrashChase() {

		//object ob = null;

		#if _DEBUG
		Debug.Log ("fine da " + stateName);
		#endif

		//return ob;
		if (lostInFront) {
			
			BasicMessageFSM pame = new BasicMessageFSM("Suspicious");

			addFinalizeMessage(pame);

		}

		lostInFront = false;

		losingInFront = false;

		_StopCoroutine (lostCor);

		i_charged (false);

	}

	IEnumerator countDownLostInFront(float _seconds) {

		yield return new WaitForSeconds(_seconds);

		lostInFront = true;

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

	private bool isTargetNotInFrontAnymore(){
		
		RaycastHit2D hit;
		float obstacleDist = -1.0f;
		GameObject foundTarget;
		
		//controllo se ho degli OSTACOLI in mezzo...
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, obstacleLayers);
		if (hit.collider != null) {
			obstacleDist = Vector2.Distance( hit.point, transform.position);
			#if _DEBUG
			Debug.Log ("PA -> CH - Raycast trova ostacolo : " + hit.transform.gameObject.name);
			#endif
			
		}
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 1.0f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 1.0f), Color.red);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, targetLayers);
		if (hit.collider != null) {
			
			foundTarget = hit.transform.gameObject;
			
			#if _DEBUG
			Debug.Log ("PA -> CH - Raycast trova target : " + foundTarget.name);
			#endif
			
			if(obstacleDist == -1.0f) {
				
				return false;
			}
			else {
				
				float targetDist = Vector2.Distance( hit.point, transform.position);
				if(targetDist < obstacleDist) {
					
					return false;
				}
			}
		}
		
		foundTarget = null;

		return true;
		
	}

	private bool CC2PcheckNotInFrontTarget(){

		return lostInFront;

	}

	protected void checkCrashWallCollision(Collision2D co) {

		if (co.gameObject.tag != "Player") {
			//Debug.Log ("ho crashato con " + co.gameObject.name);

			if(!isUnderMyFeet(co)) {

				//Debug.Log("mia altezza " + transform.position.y);

				wallCollision = true;

				wallCrashRepercussion(co);

			}

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
