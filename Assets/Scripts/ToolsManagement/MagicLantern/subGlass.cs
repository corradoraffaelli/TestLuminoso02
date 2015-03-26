using UnityEngine;
using System.Collections;

public class subGlass : MonoBehaviour {

	//public Sprite subGlassSprite;
	public bool taken = false;

	BoxCollider2D coll;
	SpriteRenderer spRend;

	// Use this for initialization
	void Start () {
		coll = GetComponent<BoxCollider2D> ();
		spRend = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			spRend.enabled = false;
			coll.enabled = false;
			taken = true;

		}
	}
}
