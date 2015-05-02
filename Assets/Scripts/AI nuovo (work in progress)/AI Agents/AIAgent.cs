using UnityEngine;
using System.Collections;

public class AIAgent : MonoBehaviour {

	protected StateFSM []states;

	public StateFSM.myStateName []stati;
	
	protected StateFSM.myStateName activeStateName;
	protected int activeStateIndex = 0;
	
	// Use this for initialization
	protected virtual void Start () {

		initializeStates ();
		initializeConditions ();

	}

	protected virtual void initializeConditions() {
		activeStateName = StateFSM.myStateName.Patrol;
		activeStateIndex = 0;
	}

	protected virtual void initializeStates(){

		states = new StateFSM[stati.Length];

		int count = 0;

		foreach (StateFSM.myStateName st in stati) {

			switch(st) {

			case StateFSM.myStateName.Patrol :

				PatrolFSM pFSM = new PatrolFSM(gameObject);
				states[count] = pFSM;

				break;

			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(gameObject);
				states[count] = cFSM;
				
				break;


			}

			count++;

		}

	}
	
	// Update is called once per frame
	protected virtual void Update () {

		StateFSM.myStateName actualStateName = activeStateName;

		states[activeStateIndex].myTransition (ref actualStateName);

		if (states[activeStateIndex].state != actualStateName) {

			makeTransition (actualStateName);
		
		}

		states[activeStateIndex].myUpdate ();
		
	}



	protected virtual void makeTransition(StateFSM.myStateName targetState) {

		for (int i=0;i<states.Length;i++) {

			if(states[i].state == targetState) {

				activeStateName = states[i].state;
				activeStateIndex = i;
				states[i].myInitialize();
				break;

			}

		}

	}

	protected virtual void OnCollisionEnter2D() {



	}

}
