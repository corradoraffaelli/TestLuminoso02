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
	
	void Start () {
		if (NPC == null)
			NPC = this.gameObject;
		playerAnimator = GeneralFinder.player.GetComponent<Animator> ();
		playerRigidbody = GeneralFinder.player.GetComponent<Rigidbody2D> ();
		camera = GeneralFinder.camera;
	}

	void Update () {
		if (playerColliding != playerCollidingOld) {
			if (playerColliding)
			{
				startDialogue();
			}
		}

		if (dialogueStarted) {
			cameraAlternativeMovements ();
			nextDialogueHandler ();
		}


		playerCollidingOld = playerColliding;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!dialogueConsumed && other.gameObject.tag == "Player") {
			playerColliding = true;
			dialogueConsumed = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	public void startDialogue()
	{
		if (balloonPrefab != null) {
			dialogueStarted = true;

			enablePlayerMovements(false);

			GeneralFinder.cameraMovements.enabled = false;
			differentCamera = true;

			showDialogue(actualIndex);
			actualIndex++;
		}
	}

	public void stopDialogue()
	{
		enablePlayerMovements(true);

		GeneralFinder.cameraMovements.enabled = true;
		differentCamera = false;
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

	void nextDialogueHandler()
	{
		if (GeneralFinder.inputKeeper != null) {
			if (GeneralFinder.inputKeeper.isButtonUp("Interaction")) {

				Destroy(balloonCreated);
				if (dialogueElements.Length < (actualIndex + 1))
					stopDialogue();
				else
					showDialogue(actualIndex++);
			}
		}
	}

	void showDialogue(int index)
	{
		if (dialogueElements [index] != null) {
			balloonCreated = Instantiate(balloonPrefab);
			ComicBalloonManager balloonManager = balloonCreated.GetComponent<ComicBalloonManager>();
			balloonManager.setType(ComicBalloonManager.Type.dialogue);
			if (dialogueElements [index].text != null && dialogueElements [index].text != "")
				balloonManager.setText(dialogueElements [index].text);
				
			if (!dialogueElements [index].player)
				balloonManager.setObjectToFollow(NPC);
		}
	}
}
