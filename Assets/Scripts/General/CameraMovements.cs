using UnityEngine;
using System.Collections;

public class CameraMovements : MonoBehaviour {

	public bool newImplementation = false;

	GameObject player;

	Vector3 cameraCenter;
	Vector3 beginCamera;
	public GameObject endingPoint;
	float xDistFromBeginning;
	float yDistFromBeginning;

	Vector3 playerPosition;
	Vector3 cursorWorldPosition;
	GameObject controller;
	CursorHandler CH;

	public float RatioDistanceFromPlayer = 0.3f;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		if (newImplementation) {

			controller = GameObject.FindGameObjectWithTag ("Controller");
			CH = controller.GetComponent<CursorHandler> ();
		} else {
			cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
			beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
			xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
			yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);

		}

	}

	void Update () {
		if (newImplementation) {
			cursorWorldPosition = CH.getCursorWorldPosition ();
			playerPosition = player.transform.position;

			transform.position = getCameraPosition (RatioDistanceFromPlayer, cursorWorldPosition, playerPosition);
			Debug.Log (getCameraPosition (RatioDistanceFromPlayer, cursorWorldPosition, playerPosition));

		} else {
		
			if ((Mathf.Abs (beginCamera.x - player.transform.position.x) > xDistFromBeginning) && 
				(Mathf.Abs (player.transform.position.x - endingPoint.transform.position.x) > xDistFromBeginning)) {
				Camera.main.gameObject.transform.position = new Vector3 (player.transform.position.x, Camera.main.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);
			}

			if (Mathf.Abs (beginCamera.y - player.transform.position.y) > yDistFromBeginning) {
				Camera.main.gameObject.transform.position = new Vector3 (Camera.main.gameObject.transform.position.x, player.transform.position.y, Camera.main.gameObject.transform.position.z);
			}
		}
	}

	Vector3 getCameraPosition(float distanceRatioFromPlayer, Vector3 curPos, Vector3 plPos)
	{
		float xCamera = (curPos.x - plPos.x) * distanceRatioFromPlayer + plPos.x;
		float yCamera = (curPos.y - plPos.y) * distanceRatioFromPlayer + plPos.y;
		Vector3 cameraPos = new Vector3(xCamera, yCamera, Camera.main.transform.position.z);
		Debug.Log ("x1 "+curPos.x);
		Debug.Log ("x "+xCamera);
		Debug.Log ("y "+yCamera);
		return cameraPos;
	}

}
