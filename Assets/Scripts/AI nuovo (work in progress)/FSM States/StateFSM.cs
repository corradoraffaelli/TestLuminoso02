using UnityEngine;
using System.Collections;

[System.Serializable]
public class StateFSM {

	public enum myStateName {
		Wander,
		Patrol,
		Chase,
		Attack,
		Flee,
		Stunned,
	}
	public myStateName state;

	//public GameObject AIGameObject;
	protected AIAgent agentScript;
	protected PlayerMovements playerScript;
	protected AIParameters parametersScript;

	public delegate void myStateInitialize();
	public myStateInitialize myInitialize;

	public delegate void myStateUpdate();
	public myStateUpdate myUpdate;

	public delegate void myStateFinalize();
	public myStateUpdate myFinalize;

	public delegate void myStateTransition(ref myStateName s);
	public myStateTransition myTransition;

	public StateFSM(GameObject go){

		agentScript = go.GetComponent<AIAgent> ();

		if (agentScript == null) {
			Debug.Log ("ATTENZIONE - script AIAgent non trovato");
		}

		playerScript = go.GetComponent<PlayerMovements> ();

		if (playerScript == null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
		}

		parametersScript = go.GetComponent<AIParameters> ();
		
		if (parametersScript == null) {
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
		}
	}

	

}
