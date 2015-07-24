using UnityEngine;
using System.Collections;

public class SpiralMovement : MonoBehaviour {


	public float circleSpeed = 1.0f;
	public float forwardSpeed = 1.0f;
	public float circleSize = 1.0f;
	public float circleGrowSpeed = 0.1f;

	Vector3 standardPosition;

	float xPos;
	float yPos;
	float zPos;

	float beginningTime = 0.0f;

	void Start () {
		standardPosition = transform.position;
		beginningTime = Time.time;
	}

	void Update () {
		xPos = Mathf.Sin(Time.time * circleSpeed) * circleSize;
		yPos = Mathf.Cos(Time.time * circleSpeed) * circleSize;
		zPos += forwardSpeed * Time.deltaTime;

		circleSize += circleGrowSpeed;

		transform.position = new Vector3(standardPosition.x + xPos,standardPosition.y + yPos, standardPosition.z + zPos);

		if ((Time.time - beginningTime) > 7.0f)
			this.enabled = false;
	}
}
