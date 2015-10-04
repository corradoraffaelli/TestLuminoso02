using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce l'entrata e l'uscita dalla camera oscura.
/// </summary>

//Dario

public class teleportHandler : MonoBehaviour {

	public Transform targetPosition;

	public bool invertCamera = false;

	public float zRotationCamera = 180.0f;

	public bool transitionEffect = false;

	public bool isTriggerTeleport = false;

	public GameObject []objectsToEnable;

	public GameObject []objectsToDisable;

	public GameObject manualCheckPoint;

	public bool defaultVerseRightWhenArrived = true;

	public bool disableWhenFinished = false;

	public teleportHandler tempTeleportForPlayer;

	public bool noEffectsBeforeFade = false;

	public enum SizeChange {
		BigToSmall,
		SmallToBig,
	}

	public SizeChange sizeChange;

	GameObject cameraGO;

	// Use this for initialization
	void Start () {

		cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void teleportPlayer() {

		StartCoroutine (handleTeleport (false));

	}

	public void noEffectsTeleportPlayer() {
		
		StartCoroutine (handleTeleport (true));
		
	}

	void enablePlayerMovements(bool enable)
	{
		Animator playerAnimator = GeneralFinder.player.GetComponent<Animator> ();
		Rigidbody2D playerRigidbody = GeneralFinder.player.GetComponent<Rigidbody2D> ();

		GeneralFinder.playerMovements.enabled = enable;

		if (!enable) {
			playerAnimator.SetBool ("onGround", true);
			playerAnimator.SetBool ("Running", false);
			playerRigidbody.velocity = Vector2.zero;
			playerRigidbody.isKinematic = true;
		} else {

			playerRigidbody.isKinematic = false;
		}
	}

	void enablePlayer(bool enable) {

		Vector3 originalScale = GeneralFinder.player.transform.localScale;
		
		//disattivare collider
		GeneralFinder.player.GetComponent<BoxCollider2D> ().enabled = enable;
		GeneralFinder.player.GetComponent<CircleCollider2D> ().enabled = enable;
		
		//fixare posizione e blocco input
		enablePlayerMovements (enable);

	}


	IEnumerator handleTeleport(bool noEffects) {

		SpriteRenderer srGameOver = cameraGO.GetComponentInChildren<SpriteRenderer> ();
		SpriteRenderer srPlayer = GeneralFinder.player.GetComponent<SpriteRenderer>();

		enablePlayer (false);

		Vector3 originalScale = GeneralFinder.player.transform.localScale;

		if (gameObject.GetComponent<UnlockContent> () != null) {
			
			gameObject.SendMessage("c_setPlayerColliding", false);
			
		}

		if (!noEffects && !noEffectsBeforeFade) {
			//ingrandisco un po il player (SEMPRE)
			for (int i=0; i<5; i++) {

				GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * 1.1f, GeneralFinder.player.transform.localScale.y * 1.1f, GeneralFinder.player.transform.localScale.z * 1.1f);
				yield return new WaitForSeconds (0.05f);

			}
		}

		float factor1 = 1.0f;
		float factor2 = 1.0f;
		float factor3 = 1.0f;

		switch (sizeChange) {

		case SizeChange.BigToSmall:
			factor1 = 0.85f;
			break;

		case SizeChange.SmallToBig:
			factor1 = 1.1f;
			break;

		}

		float startPlayerX = GeneralFinder.player.transform.position.x;

		float startPlayerY = GeneralFinder.player.transform.position.y;

		float finalPlayerX = transform.position.x;

		float finalPlayerY = transform.position.y;

		//lerpo la posizione del player verso la camera oscura e lo schiarisco
		//cambio la size del player (INGRANDISCO/RIMPICCIOLISCO)

		if (!noEffects && !noEffectsBeforeFade) {

			for (int i=0; i<10; i++) {
			
				GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * factor1, GeneralFinder.player.transform.localScale.y * factor1, GeneralFinder.player.transform.localScale.z * factor1);

				switch (sizeChange) {
				
				case SizeChange.BigToSmall:
					float deltaPosX = Mathf.Lerp (startPlayerX, finalPlayerX, ((float)(i + 1)) / ((float)10));
				
					float deltaPosY;
				
					if (i < 5) {
						deltaPosY = Mathf.Lerp (startPlayerY, finalPlayerY + 1.0f, ((float)(i + 1)) / ((float)5));
					} else {
						deltaPosY = Mathf.Lerp (finalPlayerY + 1.0f, finalPlayerY, ((float)(i - 4)) / ((float)5));
					}
				
					GeneralFinder.player.transform.position = new Vector2 (deltaPosX, deltaPosY);
					break;
				
				case SizeChange.SmallToBig:

					break;
				
				}

				float alphaValue = Mathf.Lerp (1.0f, 0.0f, ((float)(i + 1)) / ((float)10));

				srPlayer.color = new Color (srPlayer.color.r, srPlayer.color.g, srPlayer.color.b, alphaValue);

				yield return new WaitForSeconds (0.08f);
			
			}
		}
		//scurisco lo schermo

		for (int i=0; i<10; i++) {

			if (srGameOver != null) {
				
				srGameOver.color = new Color(srGameOver.color.r, srGameOver.color.g, srGameOver.color.b, ((float)i+1)/10);
				//sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/20);
				
			}
			
			yield return new WaitForSeconds(0.05f);

		}

		//TUTTO NERO

		if (invertCamera) {
			
			Transform cameraTransf = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
			
			cameraTransf.localEulerAngles = new Vector3(cameraTransf.localRotation.x, cameraTransf.localRotation.y, zRotationCamera);
			
			//cameraTransf.Rotate( new Vector3(cameraTransf.localRotation.x, cameraTransf.localRotation.y, zRotationCamera));
			
		}

		//porto il player alla posizione target
		GeneralFinder.player.transform.position = targetPosition.position;

		//GeneralFinder.player.transform.localScale = originalScale;

		switch (sizeChange) {
			
		case SizeChange.BigToSmall:
			factor2 = 3.5f;
			break;
			
		case SizeChange.SmallToBig:
			factor2 = 0.4f;
			break;
			
		}

		//porto la size del player a quella desiderata (GRANDE/PICCOLO)

		GeneralFinder.player.transform.localScale = new Vector3 (originalScale.x * factor2, originalScale.y * factor2, originalScale.z * factor2);

		switch (sizeChange) {
			
		case SizeChange.BigToSmall:
			factor3 = 0.9f;
			break;
			
		case SizeChange.SmallToBig:
			factor3 = 1.1f;
			break;
			
		}


		yield return new WaitForSeconds(0.7f);

		//flippo il player verso dove deve andare

		if (GeneralFinder.playerMovements.FacingRight && !defaultVerseRightWhenArrived ||
			!GeneralFinder.playerMovements.FacingRight && defaultVerseRightWhenArrived) {
			//GeneralFinder.playerMovements.c_flip();
		}

		enableObj ();

		disableObj ();

		if (tempTeleportForPlayer != null) {
			GeneralFinder.playerMovements.tempTeleport = tempTeleportForPlayer;
		}
		//INIZIO FINE NERO

		//schiarisco lo schermo

		for (int i = 10; i>=0; i--) {


			if (srGameOver != null) {

				srGameOver.color = new Color(srGameOver.color.r, srGameOver.color.g, srGameOver.color.b, ((float)i)/10);

			}

			yield return new WaitForSeconds(0.05f);
		}

		//faccio riapparire il player

		startPlayerX = targetPosition.transform.position.x;
		startPlayerY = targetPosition.transform.position.y;

		finalPlayerX = targetPosition.transform.position.x +1.2f;
		finalPlayerY = targetPosition.transform.position.y +1.2f; 



		for (int i=0; i<10; i++) {

			GeneralFinder.player.transform.localScale = new Vector3 (GeneralFinder.player.transform.localScale.x * factor3, GeneralFinder.player.transform.localScale.y * factor3, GeneralFinder.player.transform.localScale.z * factor3);

			switch (sizeChange) {
			
			case SizeChange.BigToSmall:

				break;
			
			case SizeChange.SmallToBig:
				float deltaPosX = Mathf.Lerp (startPlayerX, finalPlayerX, ((float)(i + 1)) / ((float)10));
			
				float deltaPosY = Mathf.Lerp (startPlayerY, finalPlayerY, ((float)(i + 1)) / ((float)10));

				GeneralFinder.player.transform.position = new Vector2 (deltaPosX, deltaPosY);
				break;
			
			}

			float alphaValue = Mathf.Lerp (0.0f, 1.0f, ((float)(i + 1)) / ((float)10));
	
			srPlayer.color = new Color (srPlayer.color.r, srPlayer.color.g, srPlayer.color.b, alphaValue);

			yield return new WaitForSeconds (0.05f);
	
		}


		GeneralFinder.player.transform.localScale = originalScale;

		enablePlayer (true);

		if(disableWhenFinished)
			this.gameObject.SetActive(false);

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
