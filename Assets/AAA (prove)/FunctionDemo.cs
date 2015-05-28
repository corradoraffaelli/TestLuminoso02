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


	void Start()
	{
		componentType = Type.GetType (componentName);

		Debug.Log ("tipo componente " + componentType.ToString ());

		componentType
			.GetMethod (MethodToCall, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Invoke (ciao.GetComponent(componentType), new object[0]);
	



	}
	
	void Update()
	{
		
	}
	
	void Foo()
	{
		Debug.Log("Foo");
	}
	
	void Bar()
	{
		Debug.Log("Bar");
	}
}