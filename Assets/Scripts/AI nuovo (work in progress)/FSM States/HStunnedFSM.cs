using UnityEngine;
using System.Collections;

public class HStunnedFSM : HEnemyStateFSM {

	float startStunnedTime = 0.0f;

	//bool killingState = false;
	bool killDuringStunn = false;
	bool stunnedFinish = false;

	int stateAfterStunnedID = -1;

	IEnumerator stunnedCor;

	StunnedParameters stunnedPar;

	protected float tStunnedLength {
		get{ 
			if(stunnedPar!=null) return stunnedPar.tStunnedLength;
			else return 0.0f;}
		set{ if(stunnedPar!=null) stunnedPar.tStunnedLength = value;}
		
	}

	public HStunnedFSM(int _stateId, GameObject _gameo, int _hLevel, HEnemyStateFSM _fatherState, AIAgent1 _scriptAIAgent, bool _killingState=true) 
	: base("Stunned", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		//killingState = _killingState;

		if (!_killingState) {
			myInitialize += normalStunnedInitialize;
		} 
		else {
			myInitialize += killingStunnedInitialize;
		}

		myUpdate += stunnedUpdate;

		myFinalize += stunnedFinalize;

		initializeStunnedParameters ();
		
	}

	protected void initializeStunnedParameters(){
		
		stunnedPar = gameObject.GetComponent<AIParameters> ().stunnedParameters;


	}

	public void setDefaultTransitions(HPatrolFSM patrolState) {


		//addTransition (S2PcountDownStunned, "Patrol");

		addTransition (S2PcountDownStunned, patrolState);

		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			

			
		}
		*/
		
	}

	public void setDefaultTransitions(HPatrol1FSM patrolState) {
		
		
		//addTransition (S2PcountDownStunned, "Patrol");
		
		addTransition (S2PcountDownStunned, patrolState);
		
		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			

			
		}
		*/
		
	}

	public void setDefaultTransitions(HWanderFSM wanderState) {
		
		//addTransition (S2PcountDownStunned, "Wander");
		addTransition (S2PcountDownStunned, wanderState);

		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			
			addTransition (S2PcountDownStunned, "Wander");
			
		}
		*/
		
	}

	protected void normalStunnedInitialize(){

		#if _DEBUG
				Debug.Log ("inizio stunn --------------------------------");
		#endif

		if (_instantKill) {
			Debug.Log(gameObject.name + "init stunn");
			i_stunned (true);
			
			handleKillo ();
			
			setDeadLayer ();
			_instantKill = false;

			if (audioHandler != null) {
				Debug.Log ("SUONO MORTE");
				audioHandler.playClipByName ("Morte");
			}
			else
				Debug.Log ("audio handler NULL");


		}
		else {
			i_stunned (true);

			startStunnedTime = Time.time;

			setEnemiesStunnedLayer ();

			stunnedFinish = false;

			stunnedCor = stunnedCountDown ();

			if (audioHandler != null) {
				Debug.Log ("SUONO MORTE");
				audioHandler.playClipByName ("Stun");
			}
			else
				Debug.Log ("audio handler NULL");

			_StartCoroutine (stunnedCor);
		}


	}

	IEnumerator stunnedCountDown() {

		yield return new WaitForSeconds (tStunnedLength);

		stunnedFinish = true;

	}

	protected void killingStunnedInitialize(){

		#if _DEBUG
		Debug.Log ("inizio stunn --------------------------------");
		#endif

		i_stunned (true);
		
		handleKillo ();

		setDeadLayer ();

		if (audioHandler != null)
			audioHandler.playClipByName ("Morte");
		else
			Debug.Log ("audio handler NULL");

	}

	protected void handleKillo() {

		_rigidbody.AddForce(new Vector2(100.0f,300.0f));
		_boxCollider.isTrigger = true;
		_circleCollider.isTrigger = true;

		checkForSpawner ();

		
	}

	private void checkForSpawner() {
		
		if (par.Spawner != null) {
			
			par.Spawner.SendMessage("letsSpawn");
			
		}
	}
	
	protected IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected void stunnedUpdate(){

		if (_instantKill && !killDuringStunn) {
			//Debug.Log("inin stunn");
			killDuringStunn = true;

			//TODO: questa parte è da rivedere?
			//caso in cui mi muoio e finisco su punte
			if(stunnedCor!=null) {
				_StopCoroutine (stunnedCor);

				i_stunned (true);
				
				handleKillo ();
				
				setDeadLayer ();

			}

		}

	}
	
	protected void stunnedFinalize(){

		#if _DEBUG
			Debug.Log ("finisco stunn --------------------------------");
		#endif

		//finishStunned = false;

		killDuringStunn = false;

		stunnedFinish = false;

		_StopCoroutine (stunnedCor);

		i_stunned (false);
		//TODO: inserire altra roba per cui serve riattivare altro...
		setDefaultLayer ();

		BasicMessageFSM pame = new BasicMessageFSM("Suspicious");
		addFinalizeMessage(pame);

		//return null;
	}

	public bool S2PcountDownStunned(){

		if(stunnedFinish) {
			par.stunnedReceived = false;
			stunnedFinish = false;
			return true;
		}
		else {
			return false;
		}

	}


}
