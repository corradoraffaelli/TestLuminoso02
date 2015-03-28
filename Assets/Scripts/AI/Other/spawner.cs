using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour {

	// Use this for initialization

	public GameObject mold;
	private GameObject myMold;
	private string moldName;
	int number = 0;

	void Start () {
		if (mold != null) {
			moldName = mold.name;
			moldName = moldName + " - ";
			myMold = (GameObject) Instantiate(mold, transform.position, Quaternion.identity);
			//myMold.transform.localScale = new Vector2(Mathf.Abs(myMold.transform.localScale.x), Mathf.Abs(myMold.transform.localScale.y));
			myMold.SetActive(false);
		}
		//StartCoroutine (createMold());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator createMold() {

		yield return new WaitForSeconds(0.5f);

		if (mold != null) {
			myMold = (GameObject) Instantiate(mold, transform.position, Quaternion.identity);
			//myMold.transform.localScale = new Vector2(Mathf.Abs(myMold.transform.localScale.x), Mathf.Abs(myMold.transform.localScale.y));
			myMold.SetActive(false);
			myMold.name = moldName;
		}
	}

	public void letsSpawn() {
		//Debug.Log ("spawno");
		GameObject newGO = Instantiate (myMold);
		newGO.SetActive (true);
		newGO.name = moldName + number++;

	}
}
