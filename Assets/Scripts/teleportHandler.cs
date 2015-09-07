using UnityEngine;
using System.Collections;

public class teleportHandler : MonoBehaviour {

	public Transform targetPosition;

	public bool invertCamera = false;

	public float zRotationCamera = 180.0f;

	public bool transitionEffect = false;

	public bool isTriggerTeleport = false;

	public GameObject []objectsToEnable;

	public GameObject []objectsToDisable;

	public GameObject manualCheckPoint;

	public bool bigToSmall = false;

	public bool smallToBig = false;

	GameObject cameraGO;

	// Use this for initialization
	void Start () {

		cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");

		if (bigToSmall && smallToBig) {

			bigToSmall = false;
			smallToBig = false;

		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void teleportPlayer() {



		StartCoroutine (handleTeleport ());


	}

	IEnumerator handleTeleport() {

		enableObj ();

		Vector3 originalScale = GeneralFinder.player.transform.localScale;

		for (int i=0; i<10; i++) {

			GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * 1.1f, GeneralFinder.player.transform.localScale.y * 1.1f, GeneralFinder.player.transform.localScale.z * 1.1f);
			yield return new WaitForSeconds(0.05f);

		}

		SpriteRenderer srGameOver = cameraGO.GetComponentInChildren<SpriteRenderer> ();

		float factor1 = 1.0f;
		float factor2 = 1.0f;
		float factor3 = 1.0f;


		if (bigToSmall) {

			factor1 = 0.85f;

		} else {
			if(smallToBig) {

				factor1 = 1.1f;

			}

		}

		for (int i=0; i<10; i++) {
			
			GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * factor1, GeneralFinder.player.transform.localScale.y * factor1, GeneralFinder.player.transform.localScale.z * factor1);

			if (srGameOver != null) {
									
				srGameOver.color = new Color(srGameOver.color.r, srGameOver.color.g, srGameOver.color.b, ((float)i+1)/10);
				//sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/20);
			}

			yield return new WaitForSeconds(0.05f);
			
		}

		if (invertCamera) {
			
			Transform cameraTransf = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
			
			cameraTransf.localEulerAngles = new Vector3(cameraTransf.localRotation.x, cameraTransf.localRotation.y, zRotationCamera);
			
			//cameraTransf.Rotate( new Vector3(cameraTransf.localRotation.x, cameraTransf.localRotation.y, zRotationCamera));
			
		}

		GeneralFinder.player.transform.position = targetPosition.position;

		//GeneralFinder.player.transform.localScale = originalScale;

		if (bigToSmall) {
			
			factor2 = 3.5f;
			
		} else {
			if(smallToBig) {
				
				factor2 = 0.4f;
				
			}
			
		}

		GeneralFinder.player.transform.localScale = new Vector3 (originalScale.x * factor2, originalScale.y * factor2, originalScale.z * factor2);

		if (bigToSmall) {
			
			factor3 = 0.9f;
			
		} else {
			if(smallToBig) {
				
				factor3 = 1.1f;
				
			}
			
		}

		yield return new WaitForSeconds(0.7f);

		for (int i = 10; i>=0; i--) {

			GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * factor3, GeneralFinder.player.transform.localScale.y * factor3, GeneralFinder.player.transform.localScale.z * factor3);

			if (srGameOver != null) {

				srGameOver.color = new Color(srGameOver.color.r, srGameOver.color.g, srGameOver.color.b, ((float)i)/10);

			}

			yield return new WaitForSeconds(0.05f);
		}

		GeneralFinder.player.transform.localScale = originalScale;

		disableObj ();

	}

	private void enableObj() {

		foreach (GameObject go in objectsToEnable) {
			
			go.SetActive(true);
			
		}


	}

	private void disableObj() {
		
		foreach (GameObject go in objectsToDisable) {
			
			go.SetActive(false);
			
		}
		
	}

	private void setManualCheckPoint () {

		if (manualCheckPoint != null) {

			manualCheckPoint.SendMessage("c_manualActivation");

		}

	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (!isTriggerTeleport)
			return;

		if (c.tag == "Player") {

			teleportPlayer();

		}

	}
}
