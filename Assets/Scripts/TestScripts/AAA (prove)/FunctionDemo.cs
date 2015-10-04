using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class FunctionDemo : MonoBehaviour
{
	public string MethodToCall;

	public string componentName;
	public Type componentType;
	public GameObject ciao;
	public Component co;
	Action useSkill;

	void Start()
	{

		//MethodInfo mi = skillsObject.GetType().GetMethod(functionName) as MethodInfo;

		//MethodInfo mi = co.GetType ().GetMethod (MethodToCall) as MethodInfo;

		//Type tipo = Type.GetType (componentName);



		//useSkill = ((Action)mi).CreateDelegate(typeof(Action), co);



		//Func<GameObject, bool> canIUseSkillOnThisObject = mi.CreateDelegate(typeof(Func<GameObject, bool>), skillsObject) as Func<GameObject, bool>;


		componentType = Type.GetType (componentName);

		Debug.Log ("tipo componente " + componentType.ToString ());

		componentType
			.GetMethod (MethodToCall, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Invoke (ciao.GetComponent(componentType), new object[0]);
	

		Debug.Log ("ciaone");

	}
	
	void Update()
	{
		useSkill ();
	}
	
	public void Foo()
	{
		Debug.Log("Foo");
	}
	
	public void Bar()
	{
		Debug.Log("Bar");
	}
}