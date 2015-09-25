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

	MagicLantern.lanternState lanternState;
	MagicLantern.lanternState lanternStateOLD;

	public bool active = true;

	public enum ActionOnObject{None, Enable, Disable};

	[System.Serializable]
	class ConsequencesElement{
		public GameObject gameObject;
		public string methodToCall;
		public ActionOnObject actionOnObject;
	}

	[System.Serializable]
	class TextElement{
		public MagicLantern.lanternState[] lanternStates;
		public string text = "Testo di prova";
		public string textController = "";
		public ConsequencesElement[] exitingConsequences;
		//[HideInInspector]
		public bool active = false;
		[HideInInspector]
		public bool activeOLD = false;
		[HideInInspector]
		public GameObject balloonCreated;
	}

	[SerializeField]
	TextElement[] textElements;

	void Start()
	{
		if (NPC == null)
			NPC = this.gameObject;
	}

	void Update () {
	
		if (active) {
			if (textElements.Length != 0)
			{
				lanternState = GeneralFinder.magicLanternLogic.actualState;
				
				if (playerColliding) {
					if ((lanternState != lanternStateOLD) || (playerColliding != playerCollidingOLD))
					{
						for (int i = 0; i < textElements.Length; i++)
						{
							if (textElements[i] != null)
							{
								bool actualFoundActive = false;
								
								if (textElements[i].lanternStates.Length == 0)
								{
									textElements[i].active = true;
									actualFoundActive = true;
								}
								
								for (int j = 0; j < textElements[i].lanternStates.Length; j++)
								{
									if ((textElements[i].lanternStates[j] == MagicLantern.lanternState.None)
									    || (textElements[i].lanternStates[j] == lanternState))
									{
										textElements[i].active = true;
										actualFoundActive = true;
										break;
									}
								}
								if (actualFoundActive)
									continue;
								setActiveFalse(i);
							}
							
							
						}
						
					}
				}
				
				if (playerColliding != playerCollidingOLD) {
					
					if (playerColliding)
					{
						
					}
					else
					{
						setAllActiveFalse();
					}
				}
				
				activeBehaviour ();
				
				updateActiveOLD ();
				lanternStateOLD = lanternState;
				playerCollidingOLD = playerColliding;
			}

			//non ci sono elementi nel textElements
			else
			{
				if (playerColliding != playerCollidingOLD) {
					if (playerColliding)
						showDialogue02();
					else
						Destroy(balloonCreated);
				}
				
				playerCollidingOLD = playerColliding;
			}
		}



		/*
		if (playerColliding != playerCollidingOLD) {
			if (playerColliding)
				showDialogue();
			else
				Destroy(balloonCreated);
		}

		playerCollidingOLD = playerColliding;
		*/

	}

	void setAllActiveFalse()
	{
		for (int i = 0; i < textElements.Length; i++)
			setActiveFalse (i);
	}

	void setActiveFalse(int index)
	{
		if (index < textElements.Length){
			//Debug.Log ("entrato");
			if (textElements [index] != null) {
				textElements [index].active = false;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void showDialogue(TextElement textElement, string inputText)
	{
		if (balloonPrefab != null) {
			textElement.balloonCreated = Instantiate(balloonPrefab);
			ComicBalloonManager balloonManager = textElement.balloonCreated.GetComponent<ComicBalloonManager>();
			balloonManager.setType(ComicBalloonManager.Type.dialogue);

			balloonManager.setText(inputText);
			
			balloonManager.setObjectToFollow(NPC);

			balloonManager.setDialogueContinue(false);

			balloonManager.setDialoguePosition(!balloonUnder);

			balloonManager.setDialogueDownOffset(-dialogueDownOffset);

			balloonManager.setDialogueOverOffset(dialogueOverOffset);
		}
	}

	void showDialogue02()
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

	public void enable()
	{
		active = true;
	}

	public void disable()
	{
		for (int i = 0; i < textElements.Length; i++) {
			if (textElements [i] != null) {
				Destroy(textElements[i].balloonCreated);
			}
		}
		Destroy(balloonCreated);
		active = false;
	}

	void activeBehaviour()
	{

		for (int i = 0; i < textElements.Length; i++) {
			if (textElements[i]!= null && (textElements[i].active != textElements[i].activeOLD))
			{
				if (textElements[i].active)
				{
					//Debug.Log ("Attivato "+i);
					if (!GeneralFinder.cursorHandler.useController)
						showDialogue(textElements[i], textElements[i].text);
					else
					{
						if (textElements[i].textController != "")
							showDialogue(textElements[i], textElements[i].textController);
						else
							showDialogue(textElements[i], textElements[i].text);
					}
				}
					
				else
				{
					//Debug.Log ("Disattivato "+i);
					Destroy(textElements[i].balloonCreated);
					callConsequences(textElements[i].exitingConsequences);
				}
					
			}
		}

	}

	void updateActiveOLD()
	{
		for (int i = 0; i < textElements.Length; i++) {
			if (textElements[i] != null)
				textElements[i].activeOLD = textElements[i].active;
		}
	}

	void callConsequences(ConsequencesElement[] cons)
	{
		if (cons != null) {
			for (int i = 0; i < cons.Length; i++)
			{
				if (cons[i] != null && cons[i].gameObject != null)
				{
					if (cons[i].actionOnObject == ActionOnObject.Enable)
						cons[i].gameObject.SetActive(true);
					if (cons[i].actionOnObject == ActionOnObject.Disable)
						cons[i].gameObject.SetActive(false);
					cons[i].gameObject.SendMessage(cons[i].methodToCall, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
