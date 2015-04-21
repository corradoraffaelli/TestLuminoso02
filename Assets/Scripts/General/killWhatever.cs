using UnityEngine;
using System.Collections;

public class killWhatever : MonoBehaviour {

	public bool oneKill = false;
	private bool turnOn = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter2D(Collision2D c) {

		if (!turnOn)
			return;

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemies") {

			c.gameObject.SendMessage("c_instantKill");

			if(oneKill)
				turnOn = false;

		}

	}
}
