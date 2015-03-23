using UnityEngine;
using System.Collections;

public class CameraMovements : MonoBehaviour {

	public GameObject player;

	Vector3 cameraCenter;
	Vector3 beginCamera;
	public GameObject endingPoint;
	float xDistFromBeginning;
	float yDistFromBeginning;
	
	void Start () {
		cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
		beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
		xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
		yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);
	}

	void Update () {

		if ((Mathf.Abs (beginCamera.x - player.transform.position.x) > xDistFromBeginning) && 
		    (Mathf.Abs(player.transform.position.x - endingPoint.transform.position.x) > xDistFromBeginning)) {
			Camera.main.gameObject.transform.position = new Vector3 (player.transform.position.x, Camera.main.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);
		}

		if (Mathf.Abs (beginCamera.y - player.transform.position.y) > yDistFromBeginning) {
			Camera.main.gameObject.transform.position = new Vector3 (Camera.main.gameObject.transform.position.x, player.transform.position.y, Camera.main.gameObject.transform.position.z);
		}

	}
}
