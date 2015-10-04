using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce lo stivale che cade del primo livello del 1921. Non usato.
/// </summary>

// Corrado
public class FallingBoot : MonoBehaviour {

	public Transform finalPosition;
	public float speed = 1.0f;
	bool falling = false;
	bool reached = false;

	Collider2D[] colliders;

	// Use this for initialization
	void Start () {
		colliders = transform.GetComponents<Collider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Fall ();
		if (reached) {
			setAsPlatform();
			reached = false;
		}
			
	}

	public void InteractingMethod()
	{
		falling = true;
	}

	void Fall()
	{
		if (falling) {
			transform.position = Vector2.MoveTowards(transform.position, finalPosition.position, speed*Time.deltaTime);
			if (transform.position == finalPosition.position)
				reached = true;
		}	
	}

	void setAsPlatform()
	{

		disableChildren ();
		for (int i = 0; i< colliders.Length; i++) {
			colliders [i].isTrigger = false;
			colliders[i].usedByEffector = true;
		}
		transform.GetComponent<PlatformEffector2D> ().enabled = true;
			
	}

	void disableChildren()
	{
		transform.GetChild (0).gameObject.SetActive(false);
	}
}
