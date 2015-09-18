using UnityEngine;
using System.Collections;

public class InformativeOnScreen : MonoBehaviour {

	public GameObject centerCamera;

	public Sprite[] spritesToShow;
	public GameObject spritesContainer;
	public BoxCollider2D imageCollider;
	public GameObject text;
	public string titolo;

	public string sortingNameText = "Cartelli";
	public int sortingIntText = 1;
	
	TextMesh textMesh;
	MeshRenderer textRenderer;

	bool playerColliding = false;
	bool playerCollidingOld = false;

	GameObject camera;
	bool differentCamera = false;
	bool dialogueStarted = false;
	
	[Range(1.0f, 300.0f)]
	public float cameraSpeed = 10.0f;
	[Range(1.0f, 100.0f)]
	public float smoothExitSpeed = 10.0f;

	float defaultSmooth;
	float slowSmooth = 1.0f;
	float actualSmooth;

	bool changingSmooth = false;

	SpriteRenderer spriteRenderer;
	int actualSpriteIndex = 0;

	void Start()
	{
		camera = GeneralFinder.camera;
		defaultSmooth = GeneralFinder.cameraMovements.smooth;
		if (centerCamera == null)
			centerCamera = this.gameObject;

		spriteRenderer = spritesContainer.GetComponent<SpriteRenderer> ();

		if (text != null) {
			textMesh = text.GetComponent<TextMesh> ();
			textRenderer = text.GetComponent<MeshRenderer>();

			textRenderer.sortingLayerName = sortingNameText;
			textRenderer.sortingOrder = sortingIntText;
			textMesh.text = titolo;
		}
			
	}

	void Update () {
		
		if (playerColliding != playerCollidingOld) {
			if (playerColliding)
			{
				GeneralFinder.cameraMovements.enabled = false;
			}
			else
			{
				GeneralFinder.cameraMovements.enabled = true;
			}
				
		}

		if (playerColliding) {
			cameraAlternativeMovements ();
		}
		smoothChanging ();
		
		playerCollidingOld = playerColliding;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			playerColliding = true;
			changingSmooth = false;
			actualSmooth = defaultSmooth;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			playerColliding = false;
			changingSmooth = true;
			actualSmooth = slowSmooth;
		}
	}

	//sostitusce i movimenti standard di camera, fa puntare la camera ad un game object di input
	void cameraAlternativeMovements()
	{
		if (camera != null) {
			Vector3 defPosition = camera.transform.position;
			Vector3 centerPosition = centerCamera.transform.position;
			Vector3 cameraEndingPosition = new Vector3(centerPosition.x, centerPosition.y, defPosition.z);
			camera.transform.position = Vector3.Lerp(camera.transform.position, cameraEndingPosition, Time.deltaTime * cameraSpeed / 100.0f);
		}
	}

	//appena il player esce dal trigger, la camera torna sul player in maniera troppo repentina
	//questa funzione cambia la variabile che gestisce questa velocità
	void smoothChanging()
	{
		if (changingSmooth) {
			actualSmooth = Mathf.MoveTowards(actualSmooth, defaultSmooth, Time.deltaTime * smoothExitSpeed / 10.0f);
			GeneralFinder.cameraMovements.smooth = actualSmooth;
		}
	}

	public void nextSprite()
	{
		actualSpriteIndex ++;
		if (actualSpriteIndex >= spritesToShow.Length)
			actualSpriteIndex = 0;
		if (spritesToShow [actualSpriteIndex] != null) {
			Sprite newSprite = spritesToShow[actualSpriteIndex];
			spriteRenderer.sprite = newSprite;
			changeImageSize();
		}
	}

	void changeImageSize()
	{
		if (imageCollider != null && spriteRenderer != null && spritesContainer != null) {
			//metto la scala ad 1
			spritesContainer.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

			//calcolo la nuova scala
			Bounds collBounds = imageCollider.bounds;
			Bounds spriteBounds = spriteRenderer.bounds;
			float newx = collBounds.size.x / spriteBounds.size.x;
			float newy = collBounds.size.y / spriteBounds.size.y;

			spritesContainer.transform.localScale = new Vector3(newx, newy, 1.0f);
		}
	}
}
