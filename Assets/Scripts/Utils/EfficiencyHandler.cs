using UnityEngine;
using System.Collections;

/// <summary>
/// Efficiency handler.
/// Ha lo scopo di attivare e disattivare gli script di un gameobject in base alla sua distanza dal player (NON USATO al momento).
/// </summary>

public class EfficiencyHandler : MonoBehaviour {

	public enum rangeSetting {
		Customized,
		TripleCameraWidth,
	}
	
	public rangeSetting rangeSet = rangeSetting.TripleCameraWidth;
	public float xRangeOfActivation = 20.0f;

	public bool cantBeStopped = false;

	public MonoBehaviour []componentsToDisable;


	bool turnOff = false;

	// Use this for initialization
	void Start () {

		StartCoroutine( initValue ());

	}


	IEnumerator initValue () {

		yield return new WaitForSeconds (0.3f);

		switch (rangeSet) {
			
		case rangeSetting.TripleCameraWidth:
			xRangeOfActivation = 3.0f * GeneralFinder.cameraHandler.getXDistFromBeginning ();
			break;
			
		case rangeSetting.Customized:
			break;
			
		default:
			break;
			
		}

	}

	// Update is called once per frame
	void Update () {

		if (componentsToDisable.Length == 0)
			return;

		if (Mathf.Abs (GeneralFinder.playerMovements.transform.position.x - this.gameObject.transform.position.x) <= xRangeOfActivation) {

			//se è già attivo, ritorna
			if(!turnOff && !cantBeStopped)
				return;

			//Debug.Log(gameObject.name + "distanza < : " + Mathf.Abs (GeneralFinder.playerMovements.transform.position.x - this.gameObject.transform.position.x) );

			foreach(MonoBehaviour script in componentsToDisable) {

				if(script != null) {
					if(!script.enabled) {

						script.enabled = true;

					}
				}
			}

			turnOff = false;

			if(cantBeStopped) {
				//Debug.Log(gameObject.name + "disabilito");
				this.enabled = false;
				
			}

		} 
		else {
			//Debug.Log(gameObject.name + "distanza > : " + Mathf.Abs (GeneralFinder.playerMovements.transform.position.x - this.gameObject.transform.position.x) );

			//se è già disattivo, ritorna
			if(turnOff)
				return;

			foreach(MonoBehaviour script in componentsToDisable) {

				if(script != null) {
					if(script.enabled) {
						
						script.enabled = false;
						
					}
				}
			}

			turnOff = true;


		}

	}

}
