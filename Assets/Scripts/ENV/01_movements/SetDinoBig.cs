using UnityEngine;
using System.Collections;

public class SetDinoBig : MonoBehaviour {

	public float finalScale = 1.0f;
	public BoxCollider2D colliderChild;
	public float yTranslation = 0.365f;

	void Start () {
	
	}

	void Update () {
	
	}

	public void setBig()
	{
		transform.localScale = new Vector3 (finalScale, finalScale, finalScale);
		colliderChild.size = new Vector2 (colliderChild.size.x / 2, colliderChild.size.y / 2);
		transform.position = new Vector3 (transform.position.x, transform.position.y + yTranslation, transform.position.z);
	}
}
