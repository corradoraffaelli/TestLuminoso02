using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class provaEvents : MonoBehaviour {


	public delegate void myUpdate ();
	public myUpdate mioUp;

	[SerializeField]
	public status mioStatus;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}




}

[System.Serializable]
public class status {

	public delegate void myUpdate ();

	[SerializeField]
	public myUpdate myUp;
	public GameObject myGo;

	public status(ref myUpdate m, GameObject go) {

		myUp = m;
		myGo = go;

	}

	public void destra() {
		
		myGo.transform.Translate (new Vector3 (1.0f, 0.0f, 0.0f));
	}

	public void alto() {
		
		myGo.transform.Translate (new Vector3 (0.0f, 1.0f, 0.0f));
	}

}