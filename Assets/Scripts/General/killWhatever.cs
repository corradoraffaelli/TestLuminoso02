using UnityEngine;
using System.Collections;

public class killWhatever : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter2D(Collision2D c) {

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemies") {

			c.gameObject.SendMessage("c_instantKill");

		}

	}
}
