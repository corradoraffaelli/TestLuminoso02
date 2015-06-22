using UnityEngine;
using System.Collections;

public class HWanderFSM : HStateFSM {

	IEnumerator flipNeedCor;
	Vector3 prevPosition;
	public int stateStunnedID = -1;

	WanderParameters wanderPar;

	protected float wanderSpeed {
		get{ 
			if(wanderPar!=null) return wanderPar.wanderSpeed;
			else return 0.0f;}
		set{ if(wanderPar!=null) wanderPar.wanderSpeed = value;}
		
	}

	public HWanderFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Wander", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		myInitialize += wanderInitialize;

		myUpdate += wanderUpdate;

		myHandleCollisionEnter += checkFlipNeedForCollision;
		myHandleCollisionEnter += checkKillPlayerCollision;

		initializeWanderParameters ();
	}

	protected void initializeWanderParameters(){
		
		wanderPar = gameObject.GetComponent<AIParameters> ().wanderParameters;
	
	}
	public void setDefaultTransitions(HStunnedFSM stunState) {
		
		//addTransition (W2ScheckStunned, "Stunned");
		addTransition (W2ScheckStunned, stunState);
	}


	protected void wanderUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (wanderSpeed);
		
	}



	protected void wanderInitialize () {

		flipNeedCor = checkFlipNeed ();

		_StartCoroutine (flipNeedCor);

	}
	
	private bool W2ScheckStunned(){

		if (par.stunnedReceived) {
			#if _DEBUG
				Debug.Log("STUNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
			#endif


			par.stunnedReceived = false;
			return true;
		} else {
			return false;
		}
	}

	private object finalizeWander() {

		_StopCoroutine (flipNeedCor);

		return null;
	}


}
