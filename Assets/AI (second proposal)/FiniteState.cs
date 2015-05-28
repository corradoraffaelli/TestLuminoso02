using UnityEngine;
using System.Collections;


public class FiniteState : MonoBehaviour {

	public string stateName = "DefaultName";

	public string initializeFunc;

	public string stateFunc;

	public string MethodToCall;

	[SerializeField]
	public TransitionHFSM []transitions;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ciao() {

		Debug.Log ("ciao");

	}

	void ciaone() {
		
		Debug.Log ("ciaone");
		
	}
}

[System.Serializable]
public class TransitionHFSM {
	
	[SerializeField]
	public string checkMethod;
	
	[SerializeField]
	public FiniteState targetState;

	public TransitionHFSM(string method, FiniteState target) {

		checkMethod = method;

		targetState = target;

	}
	
}