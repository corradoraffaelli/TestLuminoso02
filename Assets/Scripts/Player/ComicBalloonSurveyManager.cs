using UnityEngine;
using System.Collections;

public class ComicBalloonSurveyManager : MonoBehaviour {

	/*
	 * PREMESSA:
	 * Questo script è la cosa peggio fatta del mondo. è confuso e sarebbe interamente da rifare.
	 */

	public GameObject question;
	public GameObject answer1;
	public GameObject answer2;
	public GameObject answer3;
	public GameObject symbol1;
	public GameObject symbol2;
	public GameObject symbol3;

	public string questionText;
	public string answer1Text;
	public string answer2Text;
	public string answer3Text;

	Collider2D colliderQuest;
	Collider2D colliderAns1;
	Collider2D colliderAns2;
	Collider2D colliderAns3;

	BoxCollider2D boxCollider;

	SpriteRenderer rendererAns1;
	SpriteRenderer rendererAns2;
	SpriteRenderer rendererAns3;

	public string sortingLayerName = "SceneUI";
	public int sortingLayerInt = 10;

	void Start () {


	}

	void Update () {
	
	}

	void setSprites()
	{
		rendererAns1.sprite = GeneralFinder.inputManager.getSprite ("SurveyAnswer1");
		rendererAns2.sprite = GeneralFinder.inputManager.getSprite ("SurveyAnswer2");
		rendererAns3.sprite = GeneralFinder.inputManager.getSprite ("SurveyAnswer3");
	}

	public void setTexts(string[] stringhe)
	{
		questionText = stringhe [0];
		answer1Text = stringhe [1];
		answer2Text = stringhe [2];
		answer3Text = stringhe [3];
	}

	void setText(GameObject inputGO, string inputString, string sortingLayer, int sortingLayerInt)
	{
		TextMesh textMesh = inputGO.GetComponent<TextMesh> ();
		if (inputString != null && inputString != "")
		{
			inputString = inputString.Replace("NEWLINE", "\n");
			textMesh.text = inputString;
		}

		MeshRenderer textRenderer = inputGO.GetComponent<MeshRenderer> ();
		textRenderer.sortingLayerName = sortingLayer;
		textRenderer.sortingOrder = sortingLayerInt;
	}

	public BoxCollider2D createCollider()
	{
		setText (answer1, answer1Text, sortingLayerName, sortingLayerInt);
		setText (question, questionText, sortingLayerName, sortingLayerInt);
		setText (answer2, answer2Text, sortingLayerName, sortingLayerInt);
		setText (answer3, answer3Text, sortingLayerName, sortingLayerInt);
		
		rendererAns1 = symbol1.GetComponent<SpriteRenderer> ();
		rendererAns2 = symbol2.GetComponent<SpriteRenderer> ();
		rendererAns3 = symbol3.GetComponent<SpriteRenderer> ();
		
		colliderQuest = question.AddComponent<BoxCollider2D>();
		colliderQuest.isTrigger = true;
		colliderAns1 = answer1.AddComponent<BoxCollider2D>();
		colliderAns2 = answer2.AddComponent<BoxCollider2D>();
		colliderAns3 = answer3.AddComponent<BoxCollider2D>();
		colliderAns1.isTrigger = true;
		colliderAns2.isTrigger = true;
		colliderAns3.isTrigger = true;
		
		setSprites ();
		
		float minX = colliderQuest.bounds.min.x;
		if (colliderAns1.bounds.min.x < minX)
			minX = colliderAns1.bounds.min.x;
		if (colliderAns2.bounds.min.x < minX)
			minX = colliderAns2.bounds.min.x;
		if (colliderAns3.bounds.min.x < minX)
			minX = colliderAns3.bounds.min.x;
		
		float maxX = colliderQuest.bounds.max.x;
		if (colliderAns1.bounds.max.x > maxX)
			maxX = colliderAns1.bounds.max.x;
		if (colliderAns2.bounds.max.x > maxX)
			maxX = colliderAns2.bounds.max.x;
		if (colliderAns3.bounds.max.x > maxX)
			maxX = colliderAns3.bounds.max.x;
		
		float minY = colliderQuest.bounds.min.y;
		if (colliderAns1.bounds.min.y < minY)
			minY = colliderAns1.bounds.min.y;
		if (colliderAns2.bounds.min.y < minY)
			minY = colliderAns2.bounds.min.y;
		if (colliderAns3.bounds.min.y < minY)
			minY = colliderAns3.bounds.min.y;
		
		float maxY = colliderQuest.bounds.max.y;
		if (colliderAns1.bounds.max.y > maxY)
			maxY = colliderAns1.bounds.max.y;
		if (colliderAns2.bounds.max.y > maxY)
			maxY = colliderAns2.bounds.max.y;
		if (colliderAns3.bounds.max.y > maxY)
			maxY = colliderAns3.bounds.max.y;
		
		float sizeX = maxX - minX;
		float sizeY = maxY - minY;
		
		
		boxCollider = gameObject.AddComponent<BoxCollider2D>();
		boxCollider.size = new Vector2 (sizeX, sizeY);
		boxCollider.offset = new Vector2 (0.0f, sizeY / 2);
		
		question.transform.position = new Vector3 (question.transform.position.x - sizeX / 2, question.transform.position.y, question.transform.position.z);
		answer1.transform.position = new Vector3 (answer1.transform.position.x - sizeX / 2, answer1.transform.position.y, answer1.transform.position.z);
		answer2.transform.position = new Vector3 (answer2.transform.position.x - sizeX / 2, answer2.transform.position.y, answer2.transform.position.z);
		answer3.transform.position = new Vector3 (answer3.transform.position.x - sizeX / 2, answer3.transform.position.y, answer3.transform.position.z);
		symbol1.transform.position = new Vector3 (symbol1.transform.position.x - sizeX / 2, symbol1.transform.position.y, symbol1.transform.position.z);
		symbol2.transform.position = new Vector3 (symbol2.transform.position.x - sizeX / 2, symbol2.transform.position.y, symbol2.transform.position.z);
		symbol3.transform.position = new Vector3 (symbol3.transform.position.x - sizeX / 2, symbol3.transform.position.y, symbol3.transform.position.z);
		
		boxCollider.isTrigger = true;

		return boxCollider;
	}
}
