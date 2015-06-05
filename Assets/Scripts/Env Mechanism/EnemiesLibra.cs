using UnityEngine;
using System.Collections;

public class EnemiesLibra : MonoBehaviour {

	public int enemiesNumber = 4;
	int actualEnemyNumber = 0;
	GameObject[] enemiesList = new GameObject[10];

	public GameObject enemiesPlate;
	public GameObject otherPlate;

	public float deltaPos = 0.3f;
	public Vector3[] YpositionsEnemies;
	public Vector3[] YpositionsOther;

	public float speed = 0.2f;

	Collider2D objectCollider;

	// Use this for initialization
	void Start () {
		objectCollider = GetComponent<Collider2D>();

		instantiatePositions();

	}
	
	// Update is called once per frame
	void Update () {
		updatePlatformPositions();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy")
		{

			Debug.Log ("nemico in più nella bilancia");

			//controllo che non abbia già colliso con quello stesso nemico
			for (int i = 0; i<enemiesList.Length;i++)
			{
				if (enemiesList[i] != null)
					if (enemiesList[i] == other.gameObject)
						return;
			}

			//aggiungo il nemico ad una lista, giusto per tenersi un riferimento ai nemici
			enemiesList[actualEnemyNumber] = other.gameObject;

			//aumento il numero dei nemici sulla bilancia
			actualEnemyNumber++;

			//ignoro la collisione tra i nemici già sulla bilancia ed il collider della bilancia
			Collider2D[] enemyColliders = other.gameObject.GetComponents<Collider2D>();
			for (int i = 0; i< enemyColliders.Length; i++)
			{
				Physics2D.IgnoreCollision(objectCollider, enemyColliders[i]);
			}
		}
	}

	void instantiatePositions()
	{
		YpositionsEnemies = new Vector3[enemiesNumber];
		YpositionsOther = new Vector3[enemiesNumber];

		for (int i = 0; i < YpositionsEnemies.Length; i++)
		{
			float tempPos = enemiesPlate.transform.position.y - (i+1)*deltaPos;
			YpositionsEnemies[i] = new Vector3(enemiesPlate.transform.position.x, tempPos, enemiesPlate.transform.position.z);
		}

		for (int i = 0; i < YpositionsOther.Length; i++)
		{
			float tempPos = otherPlate.transform.position.y + (i+1)*deltaPos;
			YpositionsOther[i] = new Vector3(otherPlate.transform.position.x, tempPos, otherPlate.transform.position.z);
		}
	}

	void updatePlatformPositions()
	{
		if (actualEnemyNumber != 0)
		{
			enemiesPlate.transform.position = Vector3.MoveTowards(enemiesPlate.transform.position, YpositionsEnemies[actualEnemyNumber - 1], speed * Time.deltaTime);

			otherPlate.transform.position = Vector3.MoveTowards(otherPlate.transform.position, YpositionsOther[actualEnemyNumber - 1], speed * Time.deltaTime);
		}

	}
}
