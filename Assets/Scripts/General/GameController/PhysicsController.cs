using UnityEngine;
using System.Collections;

public class PhysicsController : MonoBehaviour {

	public float gravityValue = -9.81f;
	float gravity;

	bool valueChanged = false;

	// Use this for initialization
	void Start () {
		gravity = gravityValue;
		Physics2D.gravity = new Vector2 (0.0f, gravity);
	}
	
	// Update is called once per frame
	void Update () {

		//aggiorna il valore di gravità solo se modificato dall'inspector, per debug
		if (gravity != gravityValue) {
			gravity = gravityValue;
			Physics2D.gravity = new Vector2 (0.0f, gravity);
		}
	}
}
