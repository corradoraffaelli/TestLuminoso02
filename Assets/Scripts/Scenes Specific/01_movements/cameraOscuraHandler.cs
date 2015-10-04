using UnityEngine;
using System.Collections;

/// <summary>
/// Gestione camera oscura. (DEPRECATO)(NON USATO)
/// </summary>

//Dario

public class cameraOscuraHandler : MonoBehaviour {

	public GameObject []toShow;
	public GameObject[]toHide;

	public SpriteRenderer mask;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {
			Debug.Log ("ciaone1");

			Rigidbody2D rb = c.gameObject.GetComponent<Rigidbody2D>();

			rb.gravityScale = -1;

			c.gameObject.transform.localScale = new Vector3( c.gameObject.transform.localScale.x, -Mathf.Abs( c.gameObject.transform.localScale.y), c.gameObject.transform.localScale.z);
			//c.gameObject.GetComponent<PlayerMovements>().gravityMultiplier = -1;

			StartCoroutine(maskFadeIn());


		}

	}

	IEnumerator maskFadeIn() {

		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.3f);

		yield return new WaitForSeconds (0.2f);

		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.5f);

		yield return new WaitForSeconds (0.2f);
		
		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.8f);

		yield return new WaitForSeconds (0.2f);
		
		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 1.0f);

		yield return new WaitForSeconds(2.0f);


		foreach (GameObject go in toShow) {

			go.SetActive(true);

		}

		foreach (GameObject ho in toHide) {

			ho.SetActive(false);

		}


		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.8f);

		yield return new WaitForSeconds (0.2f);

		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.5f);

		yield return new WaitForSeconds (0.2f);

		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.3f);
		
		yield return new WaitForSeconds (0.2f);
		
		mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.0f);
		

	}

	public void OnTriggerExit2D(Collider2D c) {
		
		if (c.tag == "Player") {
			Debug.Log ("ciaone2");
			
			Rigidbody2D rb = c.gameObject.GetComponent<Rigidbody2D>();
			
			rb.gravityScale = 1;
			
			c.gameObject.transform.localScale = new Vector3( c.gameObject.transform.localScale.x, Mathf.Abs(c.gameObject.transform.localScale.y), c.gameObject.transform.localScale.z);
			//c.gameObject.GetComponent<PlayerMovements>().gravityMultiplier = -1;
			
			
		}
		
	}

}
