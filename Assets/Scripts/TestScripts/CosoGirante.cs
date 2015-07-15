using UnityEngine;
using System.Collections;

public class CosoGirante : MonoBehaviour {

	float actualAngle = 0.0f;
	public float speed = 10.0f;
	public float maxSpeed = 100.0f;

	void Start () {
	
	}


	void Update () {
		actualAngle = Mathf.MoveTowards(actualAngle, maxSpeed, Time.deltaTime * speed);
		transform.Rotate(new Vector3(0.0f,0.0f, actualAngle));
	}
}
