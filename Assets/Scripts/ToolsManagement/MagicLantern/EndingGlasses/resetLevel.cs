using UnityEngine;
using System.Collections;

/// <summary>
/// Caricamento di un nuovo livello all'interazione con una proiezione di fine livello. DEPRECATA.
/// </summary>

// Corrado
public class resetLevel : MonoBehaviour {

	public GameObject Q;
	SpriteRenderer QSP;
	bool colliding = false;

	// Use this for initialization
	void Start () {
		QSP = Q.GetComponent<SpriteRenderer> ();
		QSP.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (colliding) {
			QSP.enabled = true;
			if (Input.GetKeyUp (KeyCode.Q))
				Application.LoadLevel(Application.loadedLevel);

		}
		else
			QSP.enabled = false;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag.ToString() == "Player")
			colliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag.ToString() == "Player")
			colliding = false;
	}
}
