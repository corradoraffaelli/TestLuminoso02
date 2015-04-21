using UnityEngine;
using System.Collections;

public class levelChanger : MonoBehaviour {

	public bool reloadThisLevel = true;
	public string sceneName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeScene()
	{
		if (reloadThisLevel)
			Application.LoadLevel (Application.loadedLevel);
		else
			if (sceneName != "")
				Application.LoadLevel (sceneName);
	}

	public void InteractingMethod()
	{
		changeScene ();
	}
}
