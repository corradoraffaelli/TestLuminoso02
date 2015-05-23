using UnityEngine;
using System.Collections;

public class enemyTurnOffFakeLantern : MonoBehaviour {

	GameObject fakeLantern;
	FakeLanternBehaviour fakeLanternBehavious;

	void Start()
	{
		fakeLantern = transform.parent.gameObject;
		if (fakeLantern != null)
			fakeLanternBehavious = fakeLantern.GetComponent<FakeLanternBehaviour> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Enemy")
			fakeLanternBehavious.changeLanternState (FakeLanternBehaviour.fakeLanternState.Off);
	}


}
