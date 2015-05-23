using UnityEngine;
using System.Collections;

public class enemyTurnOffFakeLantern : MonoBehaviour {

	GameObject fakeLantern;
	FakeLanternBehaviour fakeLanternBehaviour;

	void Start()
	{
		fakeLantern = transform.parent.gameObject;
		if (fakeLantern != null)
			fakeLanternBehaviour = fakeLantern.GetComponent<FakeLanternBehaviour> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Enemy")
			fakeLanternBehaviour.changeLanternState (FakeLanternBehaviour.fakeLanternState.Off);
	}


}
