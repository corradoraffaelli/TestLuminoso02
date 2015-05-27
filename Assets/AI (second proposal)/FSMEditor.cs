using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Reflection;
using System;

[CustomEditor(typeof(FSM))]
public class FSMEditor : Editor
{

	int statesN = 1;

	GameObject myGameObject;

	FiniteState []fs;

	string []names;

	public void OnEnable(){
		

		
	}

	public override void OnInspectorGUI()
	{

		FSM obj = target as FSM;

		myGameObject = EditorGUILayout.ObjectField ("Obj : ", myGameObject, typeof(GameObject), true) as GameObject;
		
		if (myGameObject != null) {

			takeFiniteState();

			for (int i=0; i<names.Length; i++) {

				EditorGUILayout.TextField(names[i]);

			}

		}

	}

	void takeFiniteState(){

		Component []_finiteStatesTemp  = myGameObject.GetComponents (typeof(FiniteState));

		names = new string[_finiteStatesTemp.Length];

		int index = 0;

		foreach (Component co in _finiteStatesTemp) {

			names[index] = ((FiniteState)co).stateName;
			index++;

		}

	}

}