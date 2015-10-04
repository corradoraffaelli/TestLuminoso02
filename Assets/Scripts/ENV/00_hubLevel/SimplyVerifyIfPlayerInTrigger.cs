using UnityEngine;
using System.Collections;

/// <summary>
/// Classe da poter applicare ad un trigger, utile per sapere se il player sta collidendo.
/// </summary>

// Corrado
public class SimplyVerifyIfPlayerInTrigger : MonoBehaviour {

	bool playerColliding = false;
	public bool PlayerColliding
	{
		get{return playerColliding;}
	}


	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = true;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = false;
	}
}
