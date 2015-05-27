using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Reflection;
using System;

[CustomEditor(typeof(FiniteState))]
public class FiniteStateEditor : Editor
{
	//string[] methods;
	string myStateName;

	string[] metodi;
	string[] componentiNomi;
	string[] ignoreMethods = new string[] { "Start", "Update" };

	bool []foldOut;

	bool provando;

	bool prova;

	int transitionSize;

	GameObject myGameObject;


	FiniteState []myFiniteStates;
	string []myFiniteStatesNames;


	public void OnEnable(){

		foldOut = new bool[50];


	}

	public override void OnInspectorGUI()
	{
		
		FiniteState obj = target as FiniteState;
		
		EditorGUILayout.LabelField("Name : ");
		
		obj.stateName = EditorGUILayout.TextField (obj.stateName);

		myStateName = obj.stateName;

		//myGameObject = EditorGUILayout.ObjectField ("Obj : ", myGameObject, typeof(GameObject), true) as GameObject;
		
		takeMyMethods ();

		myGameObject = EditorGUILayout.ObjectField ("Obj : ", myGameObject, typeof(GameObject), true) as GameObject;
		
		if (myGameObject != null) {
			
			prova = EditorGUILayout.Foldout (prova, "State Methods");

			if (prova) {

				obj.initializeFunc = fillInitializeMethod (obj , "Initilize");
			
				obj.stateFunc = fillStateMethod (obj, "Update");


			}
			
			transitionSize = EditorGUILayout.IntField("Transitions ", transitionSize);
			
			if(transitionSize>0) {
				
				obj.transitions = new TransitionHFSM[transitionSize];

				takeFiniteStates();
				
				for(int i=0; i< transitionSize; i++) {
					
					foldOut[i] = EditorGUILayout.Foldout (foldOut[i], "N." + i);
					
					if(foldOut[i]) {
						
						string checkMeth = "";
						string targetS = "";

						checkMeth = EditorGUILayout.TextField (checkMeth);

						targetS = fillFiniteStateField(obj,i);
						//checkMeth = fillMethodField(obj, "Transition");
						//targetS;
						
						
						
					}
					
				}
				
			}
			
		}

	}

	void takeFiniteStates(){

		Component []_finiteStatesTemp  = myGameObject.GetComponents (typeof(FiniteState));

		myFiniteStatesNames = new string[_finiteStatesTemp.Length-1];
		myFiniteStates = new FiniteState[_finiteStatesTemp.Length-1];

		int realIndex = 0;

		for (int index = 0; index< _finiteStatesTemp.Length; index++) {

			FiniteState finst = (FiniteState) _finiteStatesTemp[realIndex];

			if(finst.stateName != myStateName) {

				myFiniteStates[realIndex] = finst;
				myFiniteStatesNames[realIndex] = finst.stateName;

				realIndex++;

			}


			//Debug.Log ("componente : " + co.name);
			
		}

	}

	string fillFiniteStateField(FiniteState obj, int targetIndex) {

		int index = 0;
		
		try {
			index = myFiniteStatesNames
				.Select ((v, i) => new { Name = v, Index = i })
					.First (x => x.Name == obj.transitions[targetIndex].targetState.stateName)
					.Index;
		} catch {
			index = 0;
		}
		
		EditorGUILayout.LabelField("Target State" + " : ");
		
		return myFiniteStatesNames [EditorGUILayout.Popup (index, myFiniteStatesNames)];

	}

	string fillInitializeMethod(FiniteState obj, string fieldName) {

		int index = 0;
		
		try {
			index = metodi
				.Select ((v, i) => new { Name = v, Index = i })
					.First (x => x.Name == obj.initializeFunc)
					.Index;
		} catch {
			index = 0;
		}
		
		EditorGUILayout.LabelField("Initilize");
		
		return metodi [EditorGUILayout.Popup (index, metodi)];
		
	}
	
	string fillStateMethod(FiniteState obj, string fieldName) {
		
		int index = 0;
		
		try {
			index = metodi
				.Select ((v, i) => new { Name = v, Index = i })
					.First (x => x.Name == obj.stateFunc)
					.Index;
		} catch {
			index = 0;
		}
		
		EditorGUILayout.LabelField("Initilize");
		
		return metodi [EditorGUILayout.Popup (index, metodi)];
		
	}

	//prende i vari componenti/script dell'oggetto
	void updateComponents(FiniteState obj) {
		
		//Component []componenti = obj.ciao.GetComponents(typeof(provainvoco));
		Component [] componenti = myGameObject.GetComponents (typeof(MonoBehaviour));
		
		componentiNomi = new string[componenti.Length];
		
		int index = 0;
		
		foreach (Component co in componenti) {
			
			Type tipo = co.GetType ();
			componentiNomi[index] = tipo.ToString();
			index++;
			//Debug.Log ("componente : " + co.name);
			
		}
		
	}

	void takeMyMethods() {

		metodi =
			typeof(FiniteState)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
				.Where(x => x.DeclaringType == typeof(FiniteState)) // Only list methods defined in our own class
				.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
				.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
				.Select(x => x.Name)
				.ToArray();

	}
	
	//una volta presi i vari script, prendiamo i vari metodi
	void takeEveryMethod()
	{
		string [] metodiTemp;
		int finalSize = 0;
		
		foreach(string s in componentiNomi) {
			
			Type tipo = Type.GetType(s);
			
			metodiTemp =
				tipo
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
					.Where(x => x.DeclaringType == tipo) // Only list methods defined in our own class
					.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
					.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
					.Select(x => x.Name)
					.ToArray();
			
			finalSize = finalSize + metodiTemp.Length;
			
		}
		
		metodi = new string[finalSize];
		
		int ind = 0;
		
		foreach(string s in componentiNomi) {
			
			Type tipo = Type.GetType(s);
			
			metodiTemp =
				tipo
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
					.Where(x => x.DeclaringType == tipo) // Only list methods defined in our own class
					.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
					.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
					.Select(x => x.Name)
					.ToArray();
			
			
			
			foreach(string m in metodiTemp) {
				
				metodi[ind] = m;
				ind++;
			}
			
		}
	}



	void getFiniteStateOfMine() {



	}













	public void OnInspectorGUI1()
	{
		
		FiniteState obj = target as FiniteState;
		
		EditorGUILayout.LabelField("Name : ");
		
		obj.stateName = EditorGUILayout.TextField ("Name");
		
		myGameObject = EditorGUILayout.ObjectField ("Obj : ", myGameObject, typeof(GameObject), true) as GameObject;
		
		if (myGameObject != null) {
			
			updateComponents(obj);
			
			takeEveryMethod ();
			
			obj.initializeFunc = fillInitializeMethod(obj, "Initilize");
			
			obj.stateFunc = fillStateMethod(obj, "Update");
			
			transitionSize = EditorGUILayout.IntField("Transitions ", transitionSize);
			
			if(transitionSize>0) {
				
				obj.transitions = new TransitionHFSM[transitionSize];
				foldOut = new bool[transitionSize];
				
				for(int i=0; i< transitionSize; i++) {
					
					foldOut[i] = EditorGUILayout.Foldout (foldOut[i], "Transition " + i + ".");
					
					if(foldOut[i]) {
						
						string checkMeth;
						string targetS;
						
						//checkMeth = fillMethodField(obj, "Transition");
						//targetS;
						
						
						
					}
					
				}
				
			}
			
		}
	}

}