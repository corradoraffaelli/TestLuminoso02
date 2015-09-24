using UnityEngine;
using System.Collections;

public class EfficiencyHandler : MonoBehaviour {

	public enum rangeSetting {
		Customized,
		CameraWidthX,
	}

	public rangeSetting rangeSet = rangeSetting.CameraWidthX;
	public float xRangeOfActivation = 20.0f;

	// Use this for initialization
	void Start () {

		switch (rangeSet) {
			case rangeSetting.CameraWidthX:
				xRangeOfActivation = GeneralFinder.cameraHandler.getXDistFromBeginning ();
				break;
			case rangeSetting.Customized:
				break;
			default:
				break;

		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Mathf.Abs (GeneralFinder.playerMovements.transform.position.x - this.gameObject.transform.position.x) <= xRangeOfActivation) {



		} else {


		}

	}

}
