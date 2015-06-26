using UnityEngine;
using System.Collections;



public class AIGuard2 : AIAgent1 {

	public enum PatrolType {
		Walk,
		Stand,
		Area,
	}
	public PatrolType patrolType;

	public AIParameters aiParam;

	protected override void initializeHStates() {
		//Time.timeScale = 0.3f;
		
		//HPatrolFSM hp = new HPatrolFSM (0, this.gameObject, 0, null, this, patrolType);

		aiParam = GetComponent<AIParameters> ();

		HPatrol1FSM hP = new HPatrol1FSM ("Patrol", this.gameObject, 0, this);

		HSuspPatrolFSM hps = new HSuspPatrolFSM (this.gameObject, 1, hP, this);
		HWalkPatrolFSM hpw = new HWalkPatrolFSM (this.gameObject, 1, hP, this);
		HPatrol1FSM defaultChildPatrol = null;

		switch(patrolType) {
			case PatrolType.Walk :
				defaultChildPatrol = hpw;
				break;
			case PatrolType.Area :
				HAreaPatrolFSM hpa = new HAreaPatrolFSM(this.gameObject, 1, hP, this);
				defaultChildPatrol = hpa;
				break;

			case PatrolType.Stand :
				HStandPatrolFSM hpst = new HStandPatrolFSM(this.gameObject, 1, hP, this);
				defaultChildPatrol = hpst;
				break;

			default:
				break;
		}

		HStunnedFSM hs =  new HStunnedFSM (1, this.gameObject, 0, null, this, false);

		HChase1FSM hc = new HChase1FSM ("Chase", this.gameObject, 0, this);
		HChargeChaseFSM hcc1 = new HChargeChaseFSM (this.gameObject, 1, hc, this);
		HCrashChaseFSM hcc2 = new HCrashChaseFSM (this.gameObject, 1, hc, this);
		
		addState (hP);
		addState (hs);
		addState (hc);

		//patrol

		hP.setDefaultStates (hps, defaultChildPatrol);
		hP.setDefaultTransitions (hs, hc);
		hP.setDefaultDelegates ();

		switch(patrolType) {
		case PatrolType.Walk :

			break;
		case PatrolType.Area :
			((HAreaPatrolFSM)defaultChildPatrol).setDefaultTransitions(hpw);
			break;
			
		case PatrolType.Stand :
			((HStandPatrolFSM)defaultChildPatrol).setDefaultTransitions(hpw);
			break;
			
		default:
			break;
		}

		hps.setDefaultTransitions (defaultChildPatrol);

		setActiveState (hP);

		//chase

		//TODO: da restringere ad una chiamata : - states - delegates - transitions - collision
		hc.setDefaultStates (hcc1, hcc2);
		hc.setDefaultInitialize ();
		hc.setDefaultTransitions (hs, hP);
		hc.setDefaultCollision ();
		
		hcc1.setDefaultTransitions (hcc2);
		hcc2.setDefaultTransitions (hs);

		//stun

		hs.setDefaultTransitions (hP);
	
		

	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Cleaner") {

			Destroy(this.gameObject);

		}

	}

	public void c_instantKill(){
		
		//TODO:
		//makeStateTransition(eMS, enemyMachineState.Stunned);
		aiParam.instantKill = true;

	}

}
