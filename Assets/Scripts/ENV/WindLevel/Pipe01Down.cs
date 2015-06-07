using UnityEngine;
using System.Collections;

public class Pipe01Down : MonoBehaviour {

	SimpleLinearMovement linearMovement;
	public GameObject[] extWindToActive;
	public GameObject[] extWindToDeactive;

	void Start()
	{
		linearMovement = GetComponent<SimpleLinearMovement>();
	}

	public void InteractingMethod()
	{
		if (linearMovement != null)
		{
			linearMovement.active = true;
		}

		for (int i = 0; i<extWindToActive.Length; i++)
		{
			if(extWindToActive[i] != null)
				extWindToActive[i].SetActive(true);
		}

		for (int i = 0; i<extWindToDeactive.Length; i++)
		{
			if(extWindToDeactive[i] != null)
				extWindToDeactive[i].SetActive(false);
		}
	}
}
