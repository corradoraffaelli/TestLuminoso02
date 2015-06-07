using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Reflection;
using System;
/*
[CustomEditor(typeof(AIAgent))]
public class AIAgentEditor : Editor {

	public int Size = 0;
	int prevSize = 0;
	//elenco degli stati possibili-> FISSO
	public string[]statesName;
	public string[][]filteredStatesName;
	//elenco degli stati scelti-> DINAMICO
	public string[]chosenStates;
	public string[]prevChosenStates;



	public void OnEnable () {

		AIAgent obj = target as AIAgent;

		Size = obj.states.Length;
		prevSize = Size;

		takeStates (obj);

		chosenStates = new string[Size];
		prevChosenStates = new string[Size];
	}

	void fillStates(AIAgent obj, int index) {


		switch(index) {
		case 0 :

			WanderFSM w = new WanderFSM();
			obj.states[index] = w;
			chosenStates[index] = w.state.ToString();
			prevChosenStates[index] = w.state.ToString();

			break;
		case 1 :
			PatrolFSM p = new PatrolFSM(PatrolFSM.patrolSubState.Walk);
			obj.states[index] = p;
			chosenStates[index] = p.state.ToString();
			prevChosenStates[index] = p.state.ToString();
			//filteredStatesName[index] = new string[Size-1];

			break;
		case 2 :
			ChaseFSM c = new ChaseFSM();
			obj.states[index] = c;
			chosenStates[index] = c.state.ToString();
			prevChosenStates[index] = c.state.ToString();

			break;
		case 3 :

			break;
		}


	}

	void allocateNewState(int index, AIAgent obj) {

		switch (chosenStates[index]) {

		case "Wander" :
			obj.states[index] = new WanderFSM();
			break;
		case "Patrol" :
			obj.states[index] = new PatrolFSM(PatrolFSM.patrolSubState.Walk);
			break;
		case "Chase":
			obj.states[index] = new ChaseFSM();
			break;
		case "Attack":
			//obj.states[index] = new AttackFSM(Selection.activeGameObject);
			break;
		case "Stunned" :
			obj.states[index] = new StunnedFSM();
			break;
		case "Flee" :
			//obj.states[index] = new FleeFSM(Selection.activeGameObject);
			break;


		}

	}

	public override void OnInspectorGUI()
	{
		
		AIAgent obj = target as AIAgent;

		Size = EditorGUILayout.IntField ("N. States : ", Size);

		if (Size <= 0) {
			obj.states = null;
			chosenStates = null;
			prevChosenStates = null;
			prevSize = 0;
			return;
		} else {

			if(prevSize==0) {
				obj.states = new StateFSM[Size];
				chosenStates = new string[Size];
				prevChosenStates = new string[Size];
				obj.states = new StateFSM[Size];
				filteredStatesName = new string[Size][];

				for(int x=0; x< Size; x++) {

					filteredStatesName[x] = new string[statesName.Length];

				}

				for(int x=0; x<Size; x++) {

					fillStates(obj, x);

				}

				prevSize = Size;
			}
			else {
				//do nothing?
				if(prevSize != Size) {
					//se è cambiata...
					//TODO: ricopiare etc etc

				}
			}
		}
		




		for(int t=0; t<Size; t++) {

			//EditorGUILayout.TextField("State : ");

			stateSelection(t, obj);

			SerializedObject subobj = new SerializedObject(obj);

			SerializedProperty property = subobj.FindProperty("ccc");

			EditorGUILayout.PropertyField(property);

			//EditorGUILayout.PropertyField(GUILayoutUtility.GetRect((float)Screen.width, EditorGUI.GetPropertyHeight(property) ), property);



		}




	}

	void stateSelection(int t, AIAgent obj) {
		
		EditorGUILayout.LabelField("Stato : ");
		
		string [] temp = new string[statesName.Length-Size+1];
		
		for(int z=0; z<temp.Length; z++) {
			
			for(int y=0; y<statesName.Length; y++) {
				
				string s = statesName[y];
				
				if(!temp.Contains<string>(s)) {
					
					if(!chosenStates.Contains<string>(s)) {
						
						//lo posso aggiungere
						temp[z] = s;
						break;
					}
					else {
						if(s == chosenStates[t]) {
							
							//lo posso aggiungere, sono io!
							temp[z] = s;
							break;
						}
						
					}
					
				}
				
			}
			
		}
		
		int index = 0;
		
		try {
			index = temp
				.Select ((v, i) => new { Name = v, Index = i })
					.First (x => x.Name == chosenStates[t])
					.Index;
		} catch {
			index = 0;
		}
		
		chosenStates[t] = temp [EditorGUILayout.Popup (index, temp )];
		
		if(prevChosenStates[t] != chosenStates[t]) {
			
			allocateNewState(t, obj);
			prevChosenStates[t] = chosenStates[t];
		}
		
	}

	void takeStates(AIAgent obj) {
		
		string [] temp = Enum.GetNames(typeof( StateFSM.myStateName) );

		statesName = temp;

	}
}
*/
