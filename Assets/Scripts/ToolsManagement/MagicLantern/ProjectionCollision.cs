using UnityEngine;
using System.Collections;

public class ProjectionCollision : MonoBehaviour {

	public GameObject projectionObject;

	GameObject colliderObject;
	bool colliding = false;

	GameObject parent;
	float scale;

	Bounds wallBounds;
	Bounds objBounds;
	
	//----DEBUG
	public GameObject cubo1;
	public GameObject cubo2;

	void Update()
	{
		//si adatta automaticamente alla nuova sprite o a cambiamenti di sprite
		objBounds = projectionObject.transform.GetComponent<SpriteRenderer> ().bounds;

		GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("ProjectionWall");

		colliding = false;

		foreach (GameObject wallObject in wallObjects) {
			wallBounds = wallObject.GetComponent<BoxCollider2D>().bounds;
			if (collisionTest(wallBounds,objBounds))
			{
				colliding = true;
				colliderObject = wallObject;
				//Debug.Log ("collidendo");
				break;
			}
		} 

		//-----------------DEBUG--------------------
		Vector3 center = projectionObject.transform.position;
		cubo1.transform.position = new Vector3 (center.x - objBounds.extents.x , center.y - objBounds.extents.y , 0.0f);
		cubo2.transform.position = new Vector3 (center.x + objBounds.extents.x , center.y + objBounds.extents.y , 0.0f);
	}

	bool collisionTest (Bounds outBounds, Bounds inBounds)
	{
		Vector3 center = projectionObject.transform.position;

		bool collide = (outBounds.min.x < center.x-inBounds.extents.x) &&
			(outBounds.min.y < center.y-inBounds.extents.y) &&
			(outBounds.max.x > center.x+inBounds.extents.x) &&
			(outBounds.max.y > center.y+inBounds.extents.y);

		return (collide);
	}

	//--------------DA TESTARE!!!!!   PROBABILMENTE INUTILE ORA COME ORA--------------------------------
	//da usare quando si cambia sprite, viene creato un buovo box collider, che si basa sulla grandezza della nuova sprite
	public void resetBoxCollider()
	{
		BoxCollider2D actualBC = projectionObject.transform.GetComponent<BoxCollider2D> ();
		Destroy (actualBC);

		BoxCollider2D newBC;
		newBC = projectionObject.gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
		newBC.isTrigger = true;
	}

	public void changeSprite(Sprite sprite)
	{
		projectionObject.transform.GetComponent<SpriteRenderer> ().sprite = sprite;
	}

	public bool isColliding()
	{
		return colliding;
	}

	public GameObject getColliderObject()
	{
		return colliderObject;
	}

	public Bounds getSpriteBounds()
	{
		return objBounds;
	}
}
