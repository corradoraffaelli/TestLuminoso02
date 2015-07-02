using UnityEngine;
using System.Collections;

public class UnlockableContentUI : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
	
	}

	public void unlockFragment(string id)
	{
		Debug.Log ("ho sbloccato il frammento "+id);
	}

	public void unlockContent(string name)
	{
		Debug.Log ("ho sbloccato l'oggetto "+name);
	}
}
