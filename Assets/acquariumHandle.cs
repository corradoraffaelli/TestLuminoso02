using UnityEngine;
using System.Collections;

public class acquariumHandle : MonoBehaviour {

	AreaEffector2D aeHorizontal;
	AreaEffector2D aeVertical;
	float maxForce = 15.0f;

	// Use this for initialization
	void Start () {
		//ae = GetComponent<AreaEffector2D> ();
		getEffectors ();

		StartCoroutine (changeVerticalWaves ());
		StartCoroutine (changeHorizontalWaves ());

	}

	private void getEffectors(){


		aeHorizontal = GetComponent<AreaEffector2D> ();

		foreach (Transform child in transform) {

			if(child.name=="Vertical") {
				
				aeVertical = child.GetComponent<AreaEffector2D> ();
				
			}

		}

	}

	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator changeVerticalWaves() {
		
		while (true) {

			yield return new WaitForSeconds (2.0f);

			aeVertical.forceMagnitude = 10.0f;

			yield return new WaitForSeconds (1.0f);
			
			aeVertical.forceMagnitude = 7.0f;

		}

	}

	private IEnumerator changeHorizontalWaves() {

		while (true) {
			yield return new WaitForSeconds (2.0f);

			if(aeHorizontal.forceMagnitude>0.0f) {
				aeHorizontal.forceMagnitude = -3.0f;
				yield return new WaitForSeconds (0.5f);
				aeHorizontal.forceMagnitude = -6.0f;
			}
			else {

				aeHorizontal.forceMagnitude = 3.0f;
				yield return new WaitForSeconds (0.5f);
				aeHorizontal.forceMagnitude = 6.0f;

			}


		}

	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {

			c.gameObject.SendMessage("c_setUnderWater", true);

		}

	}

	public void OnTriggerExit2D(Collider2D c) {

		if (c.tag == "Player") {
			
			c.gameObject.SendMessage("c_setUnderWater", false);
			
		}
		
	}


}
/*
for(int i=0;i<30;i++) {
	
	yield return new WaitForSeconds (0.1f);
	
	ae.forceMagnitude++;
	
}
*/