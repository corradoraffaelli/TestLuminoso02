using UnityEngine;
using System.Collections;

public class AIGuard2 : AIAgent1 {
	
	public HPatrolFSM.patrolSubState patrolType;
	
	protected override void initializeHStates() {
		//Time.timeScale = 0.3f;
		
		//HPatrolFSM hp = new HPatrolFSM (0, this.gameObject, 0, null, this, patrolType);

		HPatrol1FSM hP = new HPatrol1FSM ("Patrol", this.gameObject, 0, this);

		HSuspPatrolFSM hps = new HSuspPatrolFSM (this.gameObject, 1, hP, this);

		HWalkPatrolFSM hpw = new HWalkPatrolFSM (this.gameObject, 1, hP, this);

		HStunnedFSM hs =  new HStunnedFSM (1, this.gameObject, 0, null, this, false);
		//(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
		HChase1FSM hc = new HChase1FSM ("Chase", this.gameObject, 0, this);
		
		HChargeChaseFSM hcc1 = new HChargeChaseFSM (this.gameObject, 1, hc, this);
		HCrashChaseFSM hcc2 = new HCrashChaseFSM (this.gameObject, 1, hc, this);
		
		addState (hP);

		addState (hs);
		
		addState (hc);

		//patrol

		hP.setDefaultStates (hps, hpw);
		hP.setDefaultTransitions (hs, hc);
		hP.setDefaultDelegates ();
		
		hps.setDefaultTransitions (hpw);

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
}
