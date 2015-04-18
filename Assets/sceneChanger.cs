using UnityEngine;
using System.Collections;

public class sceneChanger : MonoBehaviour {

	public GameObject []toDisappear;
	public GameObject []toAppear;

	// Use this for initialization
	void Start () {
	
	}
	

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {

			foreach(GameObject go in toDisappear) {

				go.SetActive(false);

			}

			foreach(GameObject go in toAppear) {
				
				go.SetActive(true);
				
			}

			this.gameObject.SetActive(false);

		}

	}

}
