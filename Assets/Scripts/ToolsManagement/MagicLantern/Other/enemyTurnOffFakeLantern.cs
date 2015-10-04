using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce lo spegnimento della lanterna da parte di un nemico. DEPRECATA.
/// </summary>

// Corrado
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
		if (other.gameObject.tag == "Enemy" && fakeLanternBehaviour.disabledByEnemy)
			fakeLanternBehaviour.changeLanternState (FakeLanternBehaviour.fakeLanternState.Off);
	}


}
