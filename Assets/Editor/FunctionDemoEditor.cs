using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Reflection;
using System;

[CustomEditor(typeof(FunctionDemo))]
public class FunctionDemoEditor : Editor
{
	//string[] methods;
	string[] metodi;
	string[] componentiNomi;
	string[] ignoreMethods = new string[] { "Start", "Update" };

	/*
	static FunctionDemoEditor()
	{
		methods =
			typeof(FunctionDemo)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
				.Where(x => x.DeclaringType == typeof(FunctionDemo)) // Only list methods defined in our own class
				.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
				.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
				.Select(x => x.Name)
				.ToArray();

	}
	*/

	public void OnInspectorGUI2()
	{
		FunctionDemo obj = target as FunctionDemo;


		obj.ciao = EditorGUILayout.ObjectField ("Obj : ", obj.ciao, typeof(GameObject), true) as GameObject;
		/*
		if (obj != null)
		{
			int index;
			
			try
			{
				index = methods
					.Select((v, i) => new { Name = v, Index = i })
						.First(x => x.Name == obj.MethodToCall)
						.Index;
			}
			catch
			{
				index = 0;
			}
			
			obj.MethodToCall = methods[EditorGUILayout.Popup(index, methods)];
		}
		*/

		if (obj.ciao != null) {

			//SCRIPT CHOOSE------------------------
			//if(obj.componentName == null) {

			aggiornaComponenti(obj);

			//}

			int indexComponent;

			try {
				indexComponent = componentiNomi
					.Select ((v, i) => new { Name = v, Index = i })
						.First (x => x.Name == obj.componentName)
						.Index;
			} catch {
				indexComponent = 0;
			}

			EditorGUILayout.LabelField("Script : ");


			obj.componentName = componentiNomi [EditorGUILayout.Popup (indexComponent, componentiNomi)];



			//-------------------------------------

			//METHOD CHOOSE------------------------


			//if (metodi == null)
			takeMethodsFromChosenScript (obj);

			int index = 0;

			try {
				index = metodi
					.Select ((v, i) => new { Name = v, Index = i })
						.First (x => x.Name == obj.MethodToCall)
						.Index;
			} catch {
				index = 0;
			}

			EditorGUILayout.LabelField("Metodo : ");

			obj.MethodToCall = metodi [EditorGUILayout.Popup (index, metodi)];

			//-------------------------------------

		}
		else {

			metodi = null;
			
			componentiNomi = null;


		}

	}

	public override void OnInspectorGUI()
	{
		FunctionDemo obj = target as FunctionDemo;
		
		
		obj.ciao = EditorGUILayout.ObjectField ("Obj : ", obj.ciao, typeof(GameObject), true) as GameObject;
		/*
		if (obj != null)
		{
			int index;
			
			try
			{
				index = methods
					.Select((v, i) => new { Name = v, Index = i })
						.First(x => x.Name == obj.MethodToCall)
						.Index;
			}
			catch
			{
				index = 0;
			}
			
			obj.MethodToCall = methods[EditorGUILayout.Popup(index, methods)];
		}
		*/
		
		if (obj.ciao != null) {
			
			//SCRIPT CHOOSE------------------------
			//if(obj.componentName == null) {
			
			aggiornaComponenti(obj);
			
			
			//-------------------------------------
			
			//METHOD CHOOSE------------------------
			
			
			//if (metodi == null)
			takeEveryMethod ();
			
			int index = 0;
			
			try {
				index = metodi
					.Select ((v, i) => new { Name = v, Index = i })
						.First (x => x.Name == obj.MethodToCall)
						.Index;
			} catch {
				index = 0;
			}
			
			EditorGUILayout.LabelField("Metodo : ");
			
			obj.MethodToCall = metodi [EditorGUILayout.Popup (index, metodi)];
			
			//-------------------------------------
			
		}
		else {
			
			metodi = null;
			
			componentiNomi = null;
			
			
		}
		
	}

	void aggiornaComponenti(FunctionDemo obj) {

		//Component []componenti = obj.ciao.GetComponents(typeof(provainvoco));
		Component [] componenti = obj.ciao.GetComponents (typeof(MonoBehaviour));

		componentiNomi = new string[componenti.Length];

		int index = 0;

		foreach (Component co in componenti) {

			Type tipo = co.GetType ();
			componentiNomi[index] = tipo.ToString();
			index++;
			//Debug.Log ("componente : " + co.name);

		}

	}

	void takeEveryMethod()
	{
		string [] metodiTemp;
		int finalSize = 0;
		Debug.Log ("finalsize : " + finalSize);
		foreach(string s in componentiNomi) {
			Debug.Log ("s : " + s);
			//Type tipo = Type.GetType(s);
			//if(tipo==null)
			//	Debug.Log ("nullo");
			//Debug.Log ("tipo : " + tipo==null ? "null" : tipo.ToString());
			metodiTemp =
				typeof(FunctionDemo)
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
					.Where(x => x.DeclaringType == typeof(FunctionDemo)) // Only list methods defined in our own class
					.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
					.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
					.Select(x => x.Name)
					.ToArray();

			finalSize = finalSize + metodiTemp.Length;
			Debug.Log ("finalsize : " + finalSize);
		}

		metodi = new string[finalSize];

		int ind = 0;

		foreach(string s in componentiNomi) {
			
			//Type tipo = Type.GetType(s);
			
			metodiTemp =
				typeof(FunctionDemo)
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
					.Where(x => x.DeclaringType == typeof(FunctionDemo)) // Only list methods defined in our own class
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

	void takeMethodsFromChosenScript(FunctionDemo obj) {

		Type tipo = Type.GetType (obj.componentName);
		
		metodi =
			tipo
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
				.Where(x => x.DeclaringType == tipo) // Only list methods defined in our own class
				.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
				.Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
				.Select(x => x.Name)
				.ToArray();

	}
}