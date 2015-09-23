using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour {

	bool dialogueConsumed = false;

	public GameObject balloonPrefab;
	GameObject balloonCreated;

	bool playerColliding = false;
	bool playerCollidingOld = false;

	public GameObject NPC;
	//public GameObject characterObject;

	Animator playerAnimator;
	Rigidbody2D playerRigidbody;
	AudioHandler playerAudio;

	GameObject camera;
	bool differentCamera = false;
	bool dialogueStarted = false;

	int actualIndex = 0;

	[Range(1.0f, 300.0f)]
	public float cameraSpeed = 10.0f;

	[System.Serializable]
	public class DialogueElement{
		public string text = "Test";
		public bool player = true;
	}
	
	[SerializeField]
	DialogueElement[] dialogueElements;

	[SerializeField]
	DialogueElement[] dialogueElementsNext;

	public bool mustFaceRight = true;
	bool firstClick = true;

	GameObject interagibileParent;

	//float lastClick = 0.0f;
	//float diffClick = 0.3f;
	
	void Start () {
		if (NPC == null)
			NPC = this.gameObject;
		playerAnimator = GeneralFinder.player.GetComponent<Animator> ();
		playerRigidbody = GeneralFinder.player.GetComponent<Rigidbody2D> ();
		playerAudio = GeneralFinder.player.GetComponent<AudioHandler> ();
		camera = GeneralFinder.camera;

		InteragibileObject intScript = GetComponentInChildren<InteragibileObject> ();
		if (intScript != null)
			interagibileParent = intScript.gameObject;

		enableInteragibility (false);
	}

	void Update () {
		//playerColliding viene messo a true solo se è la prima volta che si collide, poi non più
		if (playerColliding != playerCollidingOld) {
			if (playerColliding)
			{
				startDialogue();
			}
		}

		if (dialogueStarted) {
			cameraAlternativeMovements ();
			if (!dialogueConsumed)
				nextDialogueHandler (dialogueElements);
			else
				nextDialogueHandler (dialogueElementsNext);

		}

		playerCollidingOld = playerColliding;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!dialogueConsumed && other.gameObject.tag == "Player") {
			playerColliding = true;
			//dialogueConsumed = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void enableInteragibility (bool enable = true)
	{
		if (interagibileParent != null)
			interagibileParent.SetActive (enable);
	}

	void facingHandler()
	{
		if ((GeneralFinder.playerMovements.FacingRight && !mustFaceRight)
			|| (!GeneralFinder.playerMovements.FacingRight && mustFaceRight))
			GeneralFinder.playerMovements.c_flip ();
	}

	public void startDialogue()
	{
		if (balloonPrefab != null) {
			dialogueStarted = true;

			if (!dialogueConsumed)
				facingHandler();
			playerAudio.stopClipByName("Passi");
			enablePlayerMovements(false);

			GeneralFinder.cameraMovements.enabled = false;
			differentCamera = true;

			if (!dialogueConsumed)
			{
				showDialogue(dialogueElements, actualIndex);
				actualIndex++;
			}


			enableInteragibility(false);
		}
	}

	public void InteractingMethod()
	{
		startDialogue ();
	}

	public void stopDialogue()
	{
		enablePlayerMovements(true);

		GeneralFinder.cameraMovements.enabled = true;
		differentCamera = false;

		dialogueStarted = false;
		dialogueConsumed = true;

		enableInteragibility(true);

		actualIndex = 0;

		firstClick = true;
	}

	void playerExit()
	{
		if (balloonCreated != null) {
			Destroy(balloonCreated);
		}
	}

	void enablePlayerMovements(bool enable)
	{
		GeneralFinder.playerMovements.enabled = enable;
		if (!enable) {
			playerAnimator.SetBool ("onGround", true);
			playerAnimator.SetBool ("Running", false);
			playerRigidbody.velocity = Vector2.zero;
		}
	}

	void cameraAlternativeMovements()
	{
		if (differentCamera && camera != null) {
			Vector3 defPosition = camera.transform.position;

			Vector3 playerPosition = GeneralFinder.player.transform.position;

			Vector3 NPCPosition = NPC.transform.position;

			Vector3 cameraEndingPosition = new Vector3((playerPosition.x + NPCPosition.x)/2, (playerPosition.y + NPCPosition.y)/2 + 1.0f, (defPosition.z));

			camera.transform.position = Vector3.Lerp(camera.transform.position, cameraEndingPosition, Time.deltaTime * cameraSpeed / 100.0f);
		}
	}

	void nextDialogueHandler(DialogueElement[] elements)
	{
		if (GeneralFinder.inputKeeper != null) {
			if (GeneralFinder.inputKeeper.isButtonUp("Interaction")) {

				Destroy(balloonCreated);
				if (dialogueElements.Length < (actualIndex + 1))
					stopDialogue();
				else
					showDialogue(elements, actualIndex++);

			}
		}
	}

	void showDialogue(DialogueElement[] elements, int index)
	{
		if (elements [index] != null) {
			balloonCreated = Instantiate(balloonPrefab);
			ComicBalloonManager balloonManager = balloonCreated.GetComponent<ComicBalloonManager>();
			balloonManager.setType(ComicBalloonManager.Type.dialogue);
			if (elements [index].text != null && elements [index].text != "")
				balloonManager.setText(elements [index].text);
				
			if (!elements [index].player)
				balloonManager.setObjectToFollow(NPC);
		}
	}
	
}
