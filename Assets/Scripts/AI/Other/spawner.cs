using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour {

	// Use this for initialization

	public GameObject mold;
	public Sprite normalS;
	public Sprite activatingS;
	[Range(1,40)]
	public int numberOfEnemies = 1;
	[Range(0.1f,10.0f)]
	public float tBeetingSpawn = 2.0f;
	private SpriteRenderer mySR;
	public GameObject myMold;
	private string moldName;

	AudioHandler audioHandler;

	public int numberOfAliveEnemies = 1;

	int number = 0;

	void Start () {

		mySR = GetComponent<SpriteRenderer> ();

		audioHandler = GetComponent<AudioHandler> ();

		setSpawnerReference ();

		StartCoroutine (createMold());
		StartCoroutine (checkSpawnNeed ());
	}

	void setSpawnerReference() {

		if (mold != null) {

			//TODO: funziona temporaneamente con entrambi i tipi di AI
			if(mold.GetComponent<basicAIEnemyV4>()!=null) {
				mold.GetComponent<basicAIEnemyV4>().Spawner = this.gameObject;
			}
			else {
				mold.GetComponent<AIParameters>().Spawner = this.gameObject;
			}
			
			moldName = mold.name;
			moldName = moldName + " - ";
			myMold = (GameObject) Instantiate(mold, transform.position, Quaternion.identity);
			//myMold.transform.localScale = new Vector2(Mathf.Abs(myMold.transform.localScale.x), Mathf.Abs(myMold.transform.localScale.y));
			myMold.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update () {

	}

	private IEnumerator checkSpawnNeed() {

		while (true) {
			yield return new WaitForSeconds(tBeetingSpawn);

			if(this.enabled) {

				if(numberOfAliveEnemies<numberOfEnemies) {

					StartCoroutine (SpawnCoroutine ());
					numberOfAliveEnemies++;

				}
			}
		}

	}

	private IEnumerator createMold() {

		yield return new WaitForSeconds(0.5f);

		if (mold != null) {
			myMold = (GameObject) Instantiate(mold, transform.position, Quaternion.identity);
			//myMold.transform.localScale = new Vector2(Mathf.Abs(myMold.transform.localScale.x), Mathf.Abs(myMold.transform.localScale.y));

			//CORRADO MOD
			SpriteRenderer moldSpriteRenderer = myMold.GetComponent<SpriteRenderer>();
			myMold.GetComponent<SpriteRenderer>().color = new Color(moldSpriteRenderer.color.r, moldSpriteRenderer.color.g, moldSpriteRenderer.color.b, 1.0f);

			myMold.SetActive(false);
			myMold.name = moldName;
		}
	}

	public void letsSpawn() {

		numberOfAliveEnemies--;

		//StartCoroutine (SpawnCoroutine ());

	}

	private IEnumerator SpawnCoroutine() {

		for (int i= 0; i<7; i++) {

			if(i==4){

				GameObject newGO = Instantiate (myMold);
				newGO.SetActive (true);
				newGO.name = moldName + number++;

				if(audioHandler!=null)
					audioHandler.playClipByName("Spawn");

			}

			yield return new WaitForSeconds (0.1f);

			mySR.sprite = activatingS;

			yield return new WaitForSeconds (0.1f);
		
			mySR.sprite = normalS;
		
		}
		
	}
}
