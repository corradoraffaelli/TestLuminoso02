using UnityEngine;
using System.Collections;

public class LanternsBehave : MonoBehaviour {

	public GameObject[] toEnable;
	int actualToEnable = 0;

	void Start () {
		
	}

	void Update () {
	
	}

	public void InteractingMethod()
	{
		if (actualToEnable<toEnable.Length && toEnable[actualToEnable] != null)
		{
			toEnable[actualToEnable].SetActive(true);
			actualToEnable++;
		}
			
	}
}
