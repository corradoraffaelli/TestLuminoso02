using UnityEngine;
using System.Collections;

public class colliderSwitch : MonoBehaviour 
{
	public bool super = false;


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			other.tag = "SuperPlayer";
			Debug.Log("Cambiato Tag in:" + other.tag);
			other.gameObject.layer = 19;
			super = true;
		}

			
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("SuperPlayer"))
		{
			other.tag = "Player";
			Debug.Log("Cambiato Tag in:" + other.tag);
			other.gameObject.layer = 12;
			super = false;
		}
			
	}

	/*void Change
	{
		if (super)


	}*/
}
