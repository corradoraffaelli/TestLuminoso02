using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayingUIGameOver : MonoBehaviour {

	GameObject canvasGameOver;
	// Use this for initialization

	void Start () {

		foreach (Transform child in transform) {

			if(child.name=="GameOverScreen") {
				canvasGameOver = child.gameObject;
				break;
			}

		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void c_GameOver() {

		if (canvasGameOver != null){

			canvasGameOver.transform.SetAsLastSibling();

			StartCoroutine (handleGameOver ());
		}
	}

	private IEnumerator handleGameOver() {
		
		Image sr = canvasGameOver.GetComponent<Image> ();
		sr.enabled = true;

		for (int i = 1; i<=20; i++) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/20);
			yield return new WaitForSeconds(0.01f);
			
		}
		
		yield return new WaitForSeconds(1.0f);
		
		for (int i = 20; i>=0; i--) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/15);
			yield return new WaitForSeconds(0.01f);
			
		}
		
		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
		sr.enabled = false;
	}

}
