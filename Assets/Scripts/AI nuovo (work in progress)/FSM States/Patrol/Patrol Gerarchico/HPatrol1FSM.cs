using UnityEngine;
using System.Collections;

public class HPatrol1FSM : HStateFSM {

	protected SpriteRenderer statusSpriteRend;

	HSuspPatrolFSM suspChildPatrolState;
	HPatrol1FSM defaultChildPatrolState;

	#region QUICKPATROLPARAMETERS
	
	public PatrolParameters patrolPar;
	
	protected float RangeOfView {
		get{ 
			if(patrolPar!=null) return patrolPar.RangeOfView;
			else return 0.0f;}
		set{
			if(patrolPar!=null) patrolPar.RangeOfView = value;}
		
	}
	
	protected GameObject patrolTarget {
		get{ 
			if(patrolPar!=null) return patrolPar.patrolTarget;
			else return null;}
		set{ if(patrolPar!=null) patrolPar.patrolTarget = value;}
		
	}
	
	protected GameObject foundTarget {
		get{ 
			if(patrolPar!=null) return patrolPar.foundTarget;
			else return null;}
		set{ if(patrolPar!=null) patrolPar.foundTarget = value;}
		
	}
	
	protected float patrolSpeed {
		get{ 
			if(patrolPar!=null) return patrolPar.patrolSpeed;
			else return 0.0f;}
		set{ if(patrolPar!=null) patrolPar.patrolSpeed = value;}
		
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
	
	#endregion QUICKPATROLPARAMETERS

	public HPatrol1FSM(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
	: base(_stateName, 0, _gameo, _hLevel, false, null, _scriptAIAgent) {

		finalHLevel = false;

		if (!getStatusSpriteRenderer (ref statusSpriteRend))
			Debug.Log ("ATTENZIONE - spriterenderer di StatusImg non trovato");

		initializePatrolParameters ();

	}

	public void setDefaultDelegates() {

		myInitialize += patrolInitialize;
		
		myFinalize += patrolFinalize;
		
		myHandleCollisionEnter += checkKillPlayerCollision;

	}

	#region INITIALIZEPATROLPARAMETERS

	public void setDefaultTransitions(HStunnedFSM stunState, HChase1FSM chaseState) {
		
		addTransition (P2ScheckStunned, stunState);
		
		addTransition (P2CcheckChaseTarget, chaseState);
	}
	

	public void setDefaultStates(HSuspPatrolFSM _suspPatrolState, HPatrol1FSM _defaultChildPatrolState) {

		suspChildPatrolState = _suspPatrolState;
		
		defaultChildPatrolState = _defaultChildPatrolState;

		addState (defaultChildPatrolState);
		addState (suspChildPatrolState);

		
	}

	protected void initializePatrolParameters(){
		
		patrolPar = gameObject.GetComponent<AIParameters> ().patrolParameters;

		float tempROV = 0;

		if (patrolPar != null) {
			if(!getRangeOfView(ref tempROV)) {
				//arbitrario valore di default 
				RangeOfView = 3.0f;
			}
			else {

				RangeOfView = tempROV;

			}
		} 
		else {
			Debug.Log ("ATTENZIONE - PatrolParameters non trovato perché non è stato trovato AIParameters");
		}
		
	}

	#endregion INITIALIZEPATROLPARAMETERS

	#region MYINITIALIZE

	protected void patrolInitialize() {
		#if _DEBUG
		Debug.Log ("inizio patrol--------------------------------");
		#endif
		/*
		if(ob!=null) {
			
			PatrolMessageFSM pame = (PatrolMessageFSM) ob;

			if(pame.getInitializationType()== "Suspicious") {

				if(suspChildPatrolState!=null)
					setActiveState(suspChildPatrolState);
				else
					Debug.Log("ATTENZIONE - suspChildPatrolState è NULL");
			}
			else {

				setActiveState(defaultChildPatrolState);

			}
		}
		*/

		//Debug.Log ("sto per prendere i messaggi - by hpatrol1");

		/*
		ArrayList tempMess = takeFinalizeMessages ();

		if (tempMess != null) {
			if(tempMess.Count>0) {

				foreach(object tempObj in tempMess) {

					BasicMessageFSM tempPame = (BasicMessageFSM)tempObj;

					if(tempPame.getInitializationType()== "Suspicious") {
						
						if(suspChildPatrolState!=null)
							setActiveState(suspChildPatrolState);
						else
							Debug.Log("ATTENZIONE - suspChildPatrolState è NULL");
					}
					else {
						
						setActiveState(defaultChildPatrolState);
						
					}

				}

			}

		}
		*/

		BasicMessageFSM []mess = takeFinalizeMessages<BasicMessageFSM> ();

		if (mess != null) {

			if(mess[0].getInitializationType()== "Suspicious") {
				
				if(suspChildPatrolState!=null)
					setActiveState(suspChildPatrolState);
				else
					Debug.Log("ATTENZIONE - suspChildPatrolState è NULL");
			}
			else {
				
				setActiveState(defaultChildPatrolState);
				
			}

		}
		
		patrolTarget = null;
		
		
	}

	#endregion MYINITIALIZE

	#region MYFINALIZE

	protected void patrolFinalize() {
		#if _DEBUG
		Debug.Log ("finisco patrol--------------------------------");
		#endif



		//object ob = null;
		
		if (foundTarget != null) {
			#if _DEBUG
			Debug.Log (" PATROL - ritorno foundtarget : " + foundTarget.name);
			#endif

			BasicMessageFSM mess = new BasicMessageFSM (foundTarget);
			
			addFinalizeMessage (mess);

		} 
		else {
			#if _DEBUG
			Debug.Log ("PATROL - ritorno NULL come foundtarget");
			#endif
			
		}

		if(defaultChildPatrolState!=null)
			setActiveState(defaultChildPatrolState);
		else
			Debug.Log("ATTENZIONE - nel finalize defaultChildPatrolState è NULL");


		//ob = (object)foundTarget;



		//return ob;
		
	}

	#endregion MYFINALIZE

	#region MYTRANSITIONS

	private bool P2CcheckChaseTarget(){
		
		RaycastHit2D hit;
		float obstacleDist = -1.0f;
		
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
				
				return true;
			}
			else {
				
				float targetDist = Vector2.Distance( hit.point, transform.position);
				if(targetDist < obstacleDist) {
					
					return true;
				}
			}
		}
		
		foundTarget = null;
		
		return false;
		
	}
	
	private bool P2ScheckStunned(){
		
		if (par.stunnedReceived) {
			
			par.stunnedReceived = false;
			return true;
		} 
		else {
			return false;
		}
	}

	#endregion MYTRANSITIONS

}

public class HSuspPatrolFSM : HPatrol1FSM {

	GameObject []suspPoints;

	IEnumerator checkAroundCor;

	bool suspFinish = false;

	bool finishSuspPatrol = false;
	float startTime = 0.0f;
	float suspLenght = 3.0f;

	float tSuspiciousLenght {
		get{ 
			if (patrolPar != null)
				return patrolPar.tSuspiciousLenght;
			else
				return 3.0f;
		}
		set {
			if (patrolPar != null)
				patrolPar.tSuspiciousLenght = value;
		
		}

	}
	
	float tSuspiciousIntervalFlip {
		get{ 
			if (patrolPar != null)
				return patrolPar.tSuspiciousIntervalFlip;
			else
				return 1.0f;
		}
		set {
			if (patrolPar != null)
				patrolPar.tSuspiciousIntervalFlip = value;
			
		}

	}

	public HSuspPatrolFSM(GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("SuspPatrol", _gameo, _hLevel, _scriptAIAgent) {
		
		finalHLevel = true;
		fatherState = _fatherState;
		myInitialize += initializeSuspPatrol;
		myFinalize += finalizeSuspPatrol;
		myUpdate += updateSuspPatrol;
		
	}

	#region MYINITIALIZE
	
	protected void initializeSuspPatrol() {
		#if _DEBUG
		Debug.Log ("inizio patrol--------------------------------");
		#endif

		//TODO: creare i due suspicious point
  		suspPoints = new GameObject[2];

		GameObject tempLeft = new GameObject ("leftSusp");
		GameObject tempRight = new GameObject ("rightSusp");
		tempLeft.transform.position = new Vector3 (transform.position.x - 1.5f, transform.position.y);
		tempRight.transform.position = new Vector3 (transform.position.x + 1.5f, transform.position.y);

		suspPoints [0] = tempLeft;
		suspPoints [1] = tempRight;

		statusSpriteRend.sprite = confusedSprite;

		startTime = Time.time;

		checkAroundCor = checkAround ();

		//_StartCoroutine (suspPatrolCor );
		_StartCoroutine (checkAroundCor);

	}

	public void setDefaultTransitions(HPatrol1FSM nextPatrolState) {

		addTransition (SP2PcheckFinishSuspPatrol, nextPatrolState);

	}
		
	
	#endregion MYINITIALIZE

	#region MYUPDATE
	
	void updateSuspPatrol(){

		//Debug.Log ("ciao da susp patrol");
		
	}

	public IEnumerator checkAround() {

		float totalTime = 0;

		while (true) {

			yield return new WaitForSeconds (tSuspiciousIntervalFlip);
			
			i_flip ();

			totalTime += tSuspiciousIntervalFlip;

			if(totalTime > tSuspiciousLenght)
				break;

		}

		suspFinish = true;

	}


	#endregion MYUPDATE

	#region MYFINALIZE
	
	protected void finalizeSuspPatrol() {

		#if _DEBUG
		Debug.Log ("finisco patrol--------------------------------");
		#endif

		_StopCoroutine (checkAroundCor);

		//object ob = null;

		GameObject.Destroy (suspPoints [0]);
		GameObject.Destroy (suspPoints [1]);

		suspPoints = null;

		statusSpriteRend.sprite = null;

		suspFinish = false;
		
	}
	
	#endregion MYFINALIZE

	#region MYTRANSITIONS
	
	private bool SP2PcheckFinishSuspPatrol(){
		/*
		if (finishSuspPatrol) {
			
			finishSuspPatrol = false;
			return true;
		} 
		else {
			return false;
		}
		*/

		if (suspFinish)
			return true;
		else
			return false;
	}
	
	#endregion MYTRANSITIONS

}


public class HWalkPatrolFSM : HPatrol1FSM {
	
	GameObject []suspPoints;
	
	IEnumerator flipNeedCor;
	Vector3 prevPosition;
	
	bool finishSuspPatrol = false;
	
	public HWalkPatrolFSM(GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("WalkPatrol", _gameo, _hLevel, _scriptAIAgent) {


		myHandleCollisionEnter += checkFlipNeedForCollision;

		finalHLevel = true;
		fatherState = _fatherState;

		myUpdate += updatePatrolWalk;

		myInitialize += initializeWalkPatrol;

		myFinalize += finalizeWalkPatrol;

	}

	void initializeWalkPatrol() {

		flipNeedCor = checkFlipNeed ();
		
		_StartCoroutine (flipNeedCor);

	}


	void updatePatrolWalk(){

		i_move (patrolSpeed);
		
	}

	void finalizeWalkPatrol() {

		//object ob = null;

		_StopCoroutine (flipNeedCor);

		//return null;

	}

	public void setDefaultTransitions() {

		
	}

}

public class HAreaPatrolFSM : HPatrol1FSM {
	
	public HAreaPatrolFSM(GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("AreaPatrol", _gameo, _hLevel, _scriptAIAgent) {

		finalHLevel = true;

		fatherState = _fatherState;
		
		myUpdate += updatePatrolArea;
	
	}

	void updatePatrolArea(){
		
		patrolBetweenPoints ();
		
	}
	
	private void patrolBetweenPoints() {
		
		if (patrolTarget == null) {
			if(patrolPar.patrolPoints.Length > 1) {
				if(patrolPar.patrolPoints[0] != null && patrolPar.patrolPoints[1]!= null) {
					int randomIndex = Random.Range(0,2);
					patrolTarget = patrolPar.patrolPoints[randomIndex];
				}
				else {
					Debug.Log ("ATTENZIONE - problemi con i patrol points");
					return;
				}
			}
			else{
				Debug.Log ("ATTENZIONE - problemi con i patrol points");
				return;
			}
		}
		
		//TODO: gestire meglio l'arrivo?
		if (Vector3.Distance (transform.position, patrolTarget.transform.position) < 0.3f) {
			
			if(patrolTarget != patrolPar.patrolPoints[0])
				patrolTarget = patrolPar.patrolPoints[0];
			else
				patrolTarget = patrolPar.patrolPoints[1];
			
		} 
		else {
			
			moveTowardTarget (patrolTarget, patrolSpeed);
			
		}
		
		
		
		
	}

}


public class HStandPatrolFSM : HPatrol1FSM {
	
	public HStandPatrolFSM(GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base ("StandPatrol", _gameo, _hLevel, _scriptAIAgent) {
		
		finalHLevel = true;
		
		fatherState = _fatherState;
		
		myUpdate += updatePatrolStand;
		
	}
	
	void updatePatrolStand() {
		
		patrolOnePoint ();
		
	}

	void patrolOnePoint() {
		
		if(patrolPar.patrolPoints.Length > 0) {
			if (Vector3.Distance (transform.position, patrolPar.patrolPoints[0].transform.position) < 0.5f) {
				
				if((!i_facingRight() && patrolPar.DefaultVerseRight == true) ||
				   (i_facingRight() && patrolPar.DefaultVerseRight == false) )
					i_flip();
				
			} 
			else {
				
				moveTowardTarget (patrolPar.patrolPoints[0], patrolSpeed);
				
			}
		}
		else {
			
			Debug.Log("ATTENZIONE - single patrol point NOT assigned");
			
		}
	}

}
