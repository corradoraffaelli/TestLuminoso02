using UnityEngine;
using System.Collections;

public class gameSet : MonoBehaviour {

	public GameObject []checkPoints;
	
	public int starterPoint = 0;

	// Use this for initialization
	void Awake () {

		checkCheckPoints ();

	}

	private void checkCheckPoints(){

		bool checkPointOk = true;

		foreach (GameObject go in checkPoints) {

			if(go==null) {
				Debug.Log ("ATTENZIONE - oggetto checkpoint nullo");
				checkPointOk = false;
			}
		}

		if (checkPointOk) {

			if(starterPoint <= 0) {

				if(starterPoint <0) {
					Debug.Log ("ATTENZIONE - starterpoint ha un valore negativo, impostato a zero, quindi punto iniziale (Vedi 'RespawnPoint' figlio di GameController)");
					starterPoint = 0;
				}

				return;
			}

			if(starterPoint > checkPoints.Length) {
				Debug.Log ("ATTENZIONE - starterpoint è troppo alto, messo al valore massimo possibile");
				starterPoint = checkPoints.Length;
				return;
			}

			for(int i=0; i<starterPoint;i++) {

				Debug.Log ("mando mess " + i);
				checkPoints[i].SendMessage("c_manualActivation");

			}


		} 
		else {
			Debug.Log ("ATTENZIONE - il settaggio della scena potrebbe non essere come desiderato");
		}

	}

	// Update is called once per frame
	void Update () {
	
	}
}
