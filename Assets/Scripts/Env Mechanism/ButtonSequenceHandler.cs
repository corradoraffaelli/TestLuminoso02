using UnityEngine;
using System.Collections;

public class ButtonSequenceHandler : MonoBehaviour {

	public enum buttonSequenceType {
		ImmediateFeedback,
		InTheEndFeedback,
	}

	public SequenceButton []buttons;

	public buttonSequenceType feedbackType;

	ChainActivationObjPiece doorScript;

	int []sequencePressed;

	int actualIndex = 0;

	public AudioHandler audioHandler;


	// Use this for initialization
	void Start () {

		initializeSequence ();

		doorScript = transform.parent.gameObject.GetComponentInChildren<ChainActivationObjPiece> ();

	}

	void initializeSequence() {

		sequencePressed = new int[buttons.Length];

		createButtons ();

	}

	// Update is called once per frame
	void Update () {
	
	}

	bool createButtons() {

		int i = 0;



		foreach (SequenceButton butt in buttons) {

			if (butt.notPushedImg == null || butt.pushedImg == null || butt.position == null || butt.sequenceNumber < 0) {
				Debug.Log (" _WARNING_ a button it's not configured in the right way");
				return false;

			}

			butt.buttObj = new GameObject ("Button " + butt.sequenceNumber);

			butt.sr = butt.buttObj.AddComponent<SpriteRenderer> ();
			butt.sr.sprite = butt.notPushedImg;

			butt.buttObj.transform.localScale = new Vector3(0.2f, 0.2f,0.2f);

			BoxCollider2D box = butt.buttObj.AddComponent<BoxCollider2D> ();

			box.size = new Vector2((box.size.x / 1.5f), (box.size.y / 2));
			box.offset = new Vector2(0.0f, -1.1f);
			box.isTrigger = true;

			butt.tsb = butt.buttObj.AddComponent<TriggerSequenceButton>();

			butt.tsb.notPushedImg = butt.notPushedImg;

			butt.tsb.pushedImg = butt.pushedImg;

			butt.tsb.sequenceNumber = butt.sequenceNumber;
			butt.tsb.handler = this.gameObject;

			butt.buttObj.transform.SetParent (transform.parent);

			butt.buttObj.transform.localPosition = butt.position.localPosition;

		}

		return true;
	
	}

	public void c_sequenceButtonPressed (int sequenceNumber) {

		switch (feedbackType) {

			case buttonSequenceType.ImmediateFeedback:
				
				immediateFeedbackEvent(sequenceNumber);
				
				break;

			case buttonSequenceType.InTheEndFeedback:
				
				lateFeedbackEvent(sequenceNumber);
				
				break;

			default :
				break;

		}



	}

	void immediateFeedbackEvent(int sequenceNumber) {

		if(checkImmediateCorrectSequence (sequenceNumber)) {


			foreach (SequenceButton butt in buttons) {
				
				if(butt.sequenceNumber == sequenceNumber) {

					
					butt.tsb.StartCoroutine(butt.tsb.animateGoToTargetPos());

				}

			}

			actualIndex++;

			if(actualIndex==buttons.Length) {

				doorScript.c_buttonPushed(true, 6.25f);
				
			}
			else {

				doorScript.c_buttonPushed(true, actualIndex);

			}
		}
		else {

			foreach (SequenceButton butt in buttons) {
				
				if(butt.sequenceNumber == sequenceNumber) {
					
					
					butt.tsb.StartCoroutine(butt.tsb.animateGoToTargetPos());
					
				}
				
			}

			resetSequence();

		}



	}

	void lateFeedbackEvent(int sequenceNumber) {

		foreach (SequenceButton butt in buttons) {
			
			if(butt.sequenceNumber == sequenceNumber) {
				
				//butt.sr.sprite = butt.pushedImg;
				
				butt.tsb.StartCoroutine(butt.tsb.animateGoToTargetPos());
				
				sequencePressed[actualIndex] = butt.sequenceNumber;
				actualIndex++;
				
				if(actualIndex==buttons.Length) {
					
					if(checkWholeCorrectSequence()) {
						
						triggerEvent();
						
					}
					else {
						
						resetSequence();
						
					}
					
					
				}
				
			}
			
		}

	}

	bool checkImmediateCorrectSequence(int pushedButtonSequenceNumber) {

		if (actualIndex == pushedButtonSequenceNumber)
			return true;
		else
			return false;

	}

	bool checkWholeCorrectSequence() {

		int prev = -1;

		foreach (int i in sequencePressed) {

			if(i<prev)
				return false;
			else
				prev = i;
		}

		return true;

	}

	void resetSequence() {

		//Debug.Log ("ERRATO");

		actualIndex = 0;

		foreach (SequenceButton butt in buttons) {

			//butt.tsb.pushed = false;

			butt.tsb.StartCoroutine(butt.tsb.animateReturnDefaultPos());

			//butt.sr.sprite = butt.notPushedImg;

		}

		if (feedbackType == buttonSequenceType.ImmediateFeedback) {

			doorScript.c_buttonPushed(false, 0);

		}

	}


	void triggerEvent() {

		Debug.Log ("BRAVO");
		gameObject.SendMessage ("c_manualActivation");

	}

}

[System.Serializable]
public class SequenceButton {

	public Sprite notPushedImg;
	public Sprite pushedImg;

	public Transform position;

	public int sequenceNumber;

	[HideInInspector]
	public GameObject buttObj;

	[HideInInspector]
	public SpriteRenderer sr;

	[HideInInspector]
	public TriggerSequenceButton tsb;

}