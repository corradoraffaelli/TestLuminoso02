using UnityEngine;
using System.Collections;

public class FakeLanternReappear : MonoBehaviour {

	public int fixedUpdateNumber = 1;
	int actualFUNumber = 0;
	bool activated = false;

	public GameObject fakeLantern;

	void Start () {
	
	}

	void Update () {
	
	}

	void FixedUpdate()
	{
		if (!activated && fakeLantern!= null)
		{
			if (actualFUNumber >= fixedUpdateNumber)
			{
				if (!fakeLantern.activeInHierarchy)
				{
					fakeLantern.SetActive(true);
					FakeLanternBehaviour behave = fakeLantern.GetComponent<FakeLanternBehaviour>();
					if (behave != null)
						behave.unlockContent = false;
				}
			}
			else
			{
				actualFUNumber++;
			}
		}
	}
}
