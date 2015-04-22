using UnityEngine;
using System.Collections;

public class DeleteFakeLantern : MonoBehaviour {

	GameObject magicLanternLogic;
	public GameObject CanvasGlasses;

	// Use this for initialization
	void Start () {
		magicLanternLogic = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
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
		gameObject.SetActive (false);

	}
}
