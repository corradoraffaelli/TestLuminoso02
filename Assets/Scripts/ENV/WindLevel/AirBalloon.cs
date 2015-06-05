using UnityEngine;
using System.Collections;

public class AirBalloon : MonoBehaviour {

	public float maxYSpeed = 1.0f;
	public float maxXSpeed = 1.0f;
	Rigidbody2D objRigidbody;

	// Use this for initialization
	void Start () {
		objRigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (objRigidbody.velocity.y > maxYSpeed)
			objRigidbody.velocity = new Vector2(objRigidbody.velocity.x, maxYSpeed);
			//objRigidbody.velocity.y = maxYSpeed;

		if (objRigidbody.velocity.x > maxXSpeed)
			objRigidbody.velocity = new Vector2(maxXSpeed, objRigidbody.velocity.y);
	}
}
