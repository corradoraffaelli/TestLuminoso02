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
		public SurveyElement survey;
		public bool player = true;
	}
	
	[SerializeField]
	DialogueElement[] dialogueElements;

	[SerializeField]
	DialogueElement[] dialogueElementsNext;

	[System.Serializable]
	public class AnswerElement{
		public string text = "The pen is on the table.";
		public bool correct = false;
	}

	[System.Serializable]
	public class SurveyElement{
		public string question = "What's your name?";
		public AnswerElement[] answers = new AnswerElement[3];
		public string correctNPCAnswer = "Bravo, risposta corretta";
		public string wrongNPCAnswer = "Sbagliato, rileggiti la scheda informativa se vuoi il regalo";
	}

	//[SerializeField]
	//SurveyElement survey;

	//public bool surveyFirstDialogue = false;
	//public bool surveyNextDialogue = false;

	public bool mustFaceRight = true;
	bool firstClick = true;

	GameObject interagibileParent;

	//float lastClick = 0.0f;
	//float diffClick = 0.3f;

	int correctAnswer;

	bool surveyStarted = false;

	public bool unlockContent = false;
	UnlockContent unlockContentScript;
	
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

		unlockContentScript = GetComponent<UnlockContent> ();
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

		if (unlockContentScript != null)
			unlockContentScript.getCollectible ();
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
				{
					if (elements[actualIndex].text != "")
					{
						showDialogue(elements, actualIndex++);
					}
					//si tratta di un questionario
					else
					{
						//if (!surveyStarted)
						//{
							surveyStarted = true;
							showSurvey(elements[actualIndex].survey, elements[actualIndex].player);
						//}
						//else
						//{
							actualIndex++;
						//}
					}
				}

			}

			/*
			if (GeneralFinder.inputManager.getButtonUp("SurveyAnswer1"))
			{
				if (correctAnswer == 0)
					showNPCAnswer(elements[actualIndex].survey, true);
				else
					showNPCAnswer(elements[actualIndex].survey, false);
			}

			if (GeneralFinder.inputManager.getButtonUp("SurveyAnswer2"))
			{
				if (correctAnswer == 1)
					showNPCAnswer(elements[actualIndex].survey, true);
				else
					showNPCAnswer(elements[actualIndex].survey, false);
			}

			if (GeneralFinder.inputManager.getButtonUp("SurveyAnswer3"))
			{
				if (correctAnswer == 2)
					showNPCAnswer(elements[actualIndex].survey, true);
				else
					showNPCAnswer(elements[actualIndex].survey, false);
			}
			*/

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

			//balloonManager.setLeftAlignment(true);
		}
	}

	void showSurvey(SurveyElement survey, bool followPlayer)
	{
		if (survey != null) {
			balloonCreated = Instantiate(balloonPrefab);
			ComicBalloonManager balloonManager = balloonCreated.GetComponent<ComicBalloonManager>();

			//randomizzo le risposte e mi salvo l'indice di quella corretta
			int firstAnswer = Random.Range (0,3);
			int secondAnswer = 10;
			int thirdAnswer = 10;
			while(secondAnswer == 10 || secondAnswer == firstAnswer)
				secondAnswer = Random.Range (0,3);
			while(thirdAnswer == 10 || thirdAnswer == firstAnswer || thirdAnswer == secondAnswer)
				thirdAnswer = Random.Range (0,3);

			int correctTempAnswer = 0;
			if (survey.answers[firstAnswer].correct)
				correctTempAnswer = firstAnswer;
			if (survey.answers[secondAnswer].correct)
				correctTempAnswer = secondAnswer;
			if (survey.answers[thirdAnswer].correct)
				correctTempAnswer = thirdAnswer;

			correctAnswer = correctTempAnswer;

			balloonManager.setSurveyTexts(survey.question, survey.answers[firstAnswer].text, survey.answers[secondAnswer].text, survey.answers[thirdAnswer].text);

			balloonManager.setSurvey(true);

			balloonManager.setType(ComicBalloonManager.Type.dialogue);
			
			if (!followPlayer)
				balloonManager.setObjectToFollow(NPC);
		}
	}

	void showNPCAnswer(SurveyElement survey, bool correct)
	{

		balloonCreated = Instantiate(balloonPrefab);
		ComicBalloonManager balloonManager = balloonCreated.GetComponent<ComicBalloonManager>();
		balloonManager.setType(ComicBalloonManager.Type.dialogue);
		if (correct)
			balloonManager.setText(survey.correctNPCAnswer);
		else
			balloonManager.setText(survey.wrongNPCAnswer);
		
		
		balloonManager.setObjectToFollow(NPC);

		actualIndex++;

	}
	
}
