using UnityEngine;
using System.Collections;

public class UnlockableContentUI : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
	
	}

	public void unlockFragment(string section, string id)
	{
		Debug.Log ("ho sbloccato il frammento "+id+ " del livello "+section);
	}

	public void unlockContent(string section, string name)
	{
		Debug.Log ("ho sbloccato l'oggetto "+name+ " del livello "+section);
	}
}
