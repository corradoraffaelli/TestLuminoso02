using UnityEngine;
using System.Collections;

public class LibraCounter : MonoBehaviour {

	public int elementsNumber = 0;
	GameObject[] collidingObject = new GameObject[10];

	//se collido con un nemico o col player, aumento la variabile che conta gli elementi
	//per evitare che vengano considerati due collider, mi tengo un array di oggetti con cui sto collidendo
	//ed ogni volta mi assicuro che l'oggetto non sia tra questi, così che i collider vengono considerati una sola volta
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player")
		{
			if (collidingObject != null)
			{
				for (int i = 0; i < collidingObject.Length; i++)
				{
					if (collidingObject[i] != null && collidingObject[i] == other.gameObject)
						return;
				}
				int index = findFirstFreeIndex();
				if (index != -1)
				{
					elementsNumber++;
					collidingObject[index] = other.gameObject;
				}
			}
		}
	}

	//ritorna l'indice della prima posizione vuota dell'array
	int findFirstFreeIndex()
	{
		if (collidingObject != null)
		{
			for (int i = 0; i < collidingObject.Length; i++)
			{
				if (collidingObject[i] == null)
					return i;
			}
		}
		return -1;
	}

	//all'uscita di un elemento, decremento la variabile
	//e tolgo l'oggetto relativo al collider dall'array degli oggetti con cui si sta collidendo
	void OnTriggerExit2D(Collider2D other)
	{
		if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player") && elementsNumber > 0)
		{
			int elementIndex = findObjectIndex(other.gameObject);
			removeElementAtIndex(elementIndex);
			elementsNumber--;
		}
	}

	//rimuove un gameObject dall'array di quelli che stanno collidendo
	void removeElementAtIndex(int inputIndex)
	{
		if (collidingObject != null)
		{
			if (inputIndex < collidingObject.Length && collidingObject[inputIndex] != null)
				collidingObject[inputIndex] = null;
		}
	}

	//ritorna l'indice del gameObject uguale a quello passato in ingresso
	int findObjectIndex(GameObject inputObject)
	{
		if (collidingObject != null)
		{
			for (int i = 0; i < collidingObject.Length; i++)
			{
				if (collidingObject[i] != null && collidingObject[i] == inputObject)
					return i;
			}
		}
		return -1;
	}
}
