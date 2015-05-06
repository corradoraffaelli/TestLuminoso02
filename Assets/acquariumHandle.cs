using UnityEngine;
using System.Collections;

public class acquariumHandle : MonoBehaviour {

	AreaEffector2D aeHorizontal;
	AreaEffector2D aeVertical;
	AreaEffector2D aeLeftBarrier;
	AreaEffector2D aeRightBarrier;
	AreaEffector2D aeBasilarSpring;

	[Range(0.1f,20.0f)]
	public float maxHorizontalForce = 8.0f;
	//public float minHorizontalForce = 5.0f;
	[Range(0.1f,5.0f)]
	public float tBetweenChangeHorForce = 0.5f;
	[Range(0.1f,20.0f)]
	public float maxVerticalForce = 10.0f;
	[Range(0.1f,5.0f)]
	public float tBetweenChangeVertForce = 0.5f;

	public bool lateralBarrier = true;
	[Range(0.1f,20.0f)]
	public float lateralBarrierForce = 10.0f;
	public bool basilarSpring = true;
	[Range(0.1f,20.0f)]
	public float basilarSpringForce = 10.0f;
	//public float minVerticalForce = 7.0f;



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

			if(child.name=="BasilarSpring") {
				
				aeBasilarSpring = child.GetComponent<AreaEffector2D> ();
				
			}

			if(child.name=="BarrierLeft") {
				
				aeLeftBarrier = child.GetComponent<AreaEffector2D> ();
				
			}

			if(child.name=="BarrierRight") {
				
				aeRightBarrier = child.GetComponent<AreaEffector2D> ();
				
			}


		}

	}

	// Update is called once per frame
	void Update () {

		if(lateralBarrier){
			aeLeftBarrier.forceMagnitude = lateralBarrierForce;
			aeRightBarrier.forceMagnitude = lateralBarrierForce;
		}
		else {

			aeLeftBarrier.forceMagnitude = 0.0f;
			aeRightBarrier.forceMagnitude = 0.0f;

		}

		if (basilarSpring) {
			aeBasilarSpring.forceMagnitude = basilarSpringForce;
		}
		else {
			aeBasilarSpring.forceMagnitude = 0.0f;

		}



	}

	private IEnumerator changeVerticalWaves() {
		
		while (true) {

			yield return new WaitForSeconds ( tBetweenChangeVertForce / 2);

			aeVertical.forceMagnitude = maxVerticalForce / 2;

			yield return new WaitForSeconds ( tBetweenChangeVertForce / 2);
			
			aeVertical.forceMagnitude = maxVerticalForce;

		}

	}

	private IEnumerator changeHorizontalWaves() {

		while (true) {
			yield return new WaitForSeconds ( tBetweenChangeHorForce );

			if(aeHorizontal.forceMagnitude>0.0f) {
				aeHorizontal.forceMagnitude = - maxHorizontalForce / 2 ;
				yield return new WaitForSeconds ( tBetweenChangeHorForce );
				aeHorizontal.forceMagnitude = - maxHorizontalForce;
			}
			else {

				aeHorizontal.forceMagnitude = maxHorizontalForce / 2;
				yield return new WaitForSeconds (tBetweenChangeHorForce);
				aeHorizontal.forceMagnitude = maxHorizontalForce;

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