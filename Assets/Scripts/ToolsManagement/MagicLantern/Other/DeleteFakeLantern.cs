using UnityEngine;
using System.Collections;

public class DeleteFakeLantern : MonoBehaviour {

	GameObject magicLanternLogic;
	public GameObject CanvasGlasses;
	AudioHandler audioHandler;

	// Use this for initialization
	void Start () {
		magicLanternLogic = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		audioHandler = GetComponent<AudioHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InteractingMethod()
	{
		if (magicLanternLogic)
			magicLanternLogic.GetComponent<MagicLantern> ().setActivable (true);
		if (CanvasGlasses)
			CanvasGlasses.SetActive (true);

		if (audioHandler != null)
			audioHandler.playClipByName ("Leva");

		//GameObject[] child = gameObject.transform.
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}

		//gameObject.SetActive (false);

	}
}
