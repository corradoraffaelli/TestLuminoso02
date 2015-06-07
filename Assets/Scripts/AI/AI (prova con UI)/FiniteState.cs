using UnityEngine;
using System.Collections;
using System;
using System.Reflection;


public class FiniteState : MonoBehaviour {

	public string stateName = "DefaultName";

	public string initializeFunc;
	public string initializeFuncScript;
	GameObject initializeFuncGameObject;

	public string updateFunc;
	public string updateFuncScript;
	GameObject updateFuncGameObject;

	public string MethodToCall;

	[SerializeField]
	public TransitionHFSM []transitions;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		checkTransitions ();

		updateState ();

	}

	void checkTransitions() {

		for (int i=0; i<transitions.Length; i++) {

			invokeMethodByName(transitions[i].checkFunc, transitions[i].checkFuncScript, null, transitions[i].checkFuncGameObject);

		}

	}

	void updateState(){

		invokeMethodByName (updateFunc, updateFuncScript, null, updateFuncGameObject);

	}

	void invokeMethodByName(string methodName) {

		invokeMethodByName(methodName, this.name);

	}

	void invokeMethodByName(string methodName, string scriptName, object []parameter = null, GameObject scriptGameObject = null){

		Type componentType = Type.GetType (scriptName);

		if (scriptGameObject == null)
			scriptGameObject = this.gameObject;

		Debug.Log ("Script : " + componentType.ToString ());
		
		componentType
			.GetMethod (MethodToCall, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Invoke (scriptGameObject.GetComponent(componentType), new object[0]);


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
	public string checkFunc;

	public string checkFuncScript;
	public GameObject checkFuncGameObject;

	[SerializeField]
	public FiniteState targetState;

	public TransitionHFSM(string checkMethod, string checkMethodScript, GameObject checkMethodGameObject, FiniteState target) {

		checkFunc = checkMethod;
		checkFuncScript = checkMethodScript;
		checkFuncGameObject = checkMethodGameObject;

		targetState = target;

	}
	
}