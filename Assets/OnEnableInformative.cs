using UnityEngine;
using System.Collections;

public class OnEnableInformative : MonoBehaviour {

	// Use this for initialization
	void OnEnable() {

		GeneralFinder.informativeManager.c_onEnableInformative ();

		GeneralFinder.informativeManager.c_setHelpImages ();

	}

}
