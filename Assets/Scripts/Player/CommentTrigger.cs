using UnityEngine;
using System.Collections;

public class CommentTrigger : MonoBehaviour {

	public GameObject balloonPrefab;
	GameObject balloonCreated;

	public GameObject NPC;
	public string text;

	public bool balloonUnder = false;
	[Range(-3.0f, 3.0f)]
	public float dialogueDownOffset = 0.0f;
	[Range(-3.0f, 3.0f)]
	public float dialogueOverOffset = 0.0f;

	bool playerColliding = false;
	bool playerCollidingOLD = false;
	
	void Start()
	{
		if (NPC == null)
			NPC = this.gameObject;
	}

	void Update () {
	
		if (playerColliding != playerCollidingOLD) {
			if (playerColliding)
				showDialogue();
			else
				Destroy(balloonCreated);
		}

		playerCollidingOLD = playerColliding;

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void showDialogue()
	{
		if (balloonPrefab != null) {
			balloonCreated = Instantiate(balloonPrefab);
			ComicBalloonManager balloonManager = balloonCreated.GetComponent<ComicBalloonManager>();
			balloonManager.setType(ComicBalloonManager.Type.dialogue);

			balloonManager.setText(text);
			
			balloonManager.setObjectToFollow(NPC);

			balloonManager.setDialogueContinue(false);

			balloonManager.setDialoguePosition(!balloonUnder);

			balloonManager.setDialogueDownOffset(-dialogueDownOffset);

			balloonManager.setDialogueOverOffset(dialogueOverOffset);
		}
	}
}
