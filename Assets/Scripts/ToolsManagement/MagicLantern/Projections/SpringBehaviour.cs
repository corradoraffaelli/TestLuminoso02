﻿using UnityEngine;
using System.Collections;

public class SpringBehaviour : MonoBehaviour {

	GameObject player;
	float vel01;
	bool scendendo = false;
	bool spinta = false;
	Rigidbody2D rigBody;
	public float upForce = 500.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		rigBody = player.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		vel01 = rigBody.velocity.y;
		if (vel01 < 0.0f)
			scendendo = true;
		else
			scendendo = false;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == player && scendendo == true)
			spinta = true;
	}

	void FixedUpdate()
	{
		if (spinta) {
			rigBody.velocity = new Vector3(rigBody.velocity.x,0.0f,0.0f);
			rigBody.AddForce(new Vector2 (0.0f,upForce));
			spinta = false;
		}
		//vel01 = player.GetComponent<Rigidbody2D> ().velocity.y;
	}
}