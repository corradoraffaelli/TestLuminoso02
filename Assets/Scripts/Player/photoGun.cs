using UnityEngine;
using System.Collections;

public class photoGun : Tool {

	float tStartShoot = -7.0f;
	float tShoot = 1.0f;
	float tBetweenFrame = 0.22f;
	float tToReactivateGun = 5.0f;
	bool shooting = false;
	int countFrames = 0;
	int maxCountFrames = 4;
	public GameObject detectedEnemies;
	//public ArrayList detectedEnemies;
	public LayerMask enemiesLayer;
	bool inAction = false;

	protected override void initializeTool() {
		
		//inventory = GameObject.Find ("Inventory");

	}

	
	protected override void useTool() {

		//transform.position = actualMousePosition;

		if (!usingOneClick) {
			transform.position = actualMousePosition;
			return;
		} else {

		}

		//Debug.Log ("uso tool");
		if (!shooting) {
			//se non è partito lo scatto, inizializzo il tutto
			if (Time.time < tStartShoot + tToReactivateGun )
				return;

			//Debug.Log ("shootùùùùùùùùùùùù");

			inAction = true;
			shooting = true;
			tStartShoot = Time.time;
		} 

		else {
			//gestisco gli scatti

			//Debug.Log ("scatto");
			if(Time.time -tStartShoot > tShoot) {
				//tempo di cattura finito

				//Debug.Log ("finishhhhhhhhh");
				shooting = false;
				countFrames = 0;
				inAction = false;
				usingOneClick = false;

			}
			else {
				//scatto

				//Debug.Log ("scatto1");
				shootArea();

			}
		}
	}

	private void i_freeze(bool c, GameObject myTarget) {

		myTarget.GetComponent<basicAIEnemyV4>().c_freeze_ai(c);

	}

	//TODO: cambiare layer alle copie
	private void frameTarget() {

		countFrames++;

		if (countFrames == maxCountFrames) {

			i_freeze (false, detectedEnemies);

		} else {
			GameObject copy = (GameObject)Instantiate ((GameObject)detectedEnemies);

			i_freeze (true, copy);

			//copy.SendMessage ("setAutoDestroy", true);

		}

		//Instantiate(detectedEnemies[0]);
	}

	private void shootArea() {
		//Debug.Log ("scatto2");
		if (detectedEnemies != null) {
			//Debug.Log ("scatto3");

			if (detectedEnemies.GetComponent<Rigidbody2D> ().velocity.y < 0.0f) {
				if(countFrames < maxCountFrames && (Time.time > tStartShoot + countFrames*tBetweenFrame*0.5f) )
					frameTarget();
				
			} 
			else {
				if(countFrames < maxCountFrames && (Time.time > tStartShoot + countFrames*tBetweenFrame) )
					frameTarget();

				//detectedEnemies[0]
			}
		}

	}

	public void OnTriggerEnter2D(Collider2D c) {
		//Debug.Log ("ciao");
		if ((enemiesLayer.value & 1 << c.gameObject.layer) > 0) {
		//if (c.gameObject.layer == 13) {
			/*
			foreach(GameObject g in detectedEnemies) {

				if(g == c.gameObject) {
					return;
				}

			}
			*/
			//Debug.Log("ho beccato" + c.gameObject.name);
			if(detectedEnemies != null)
				return;

			detectedEnemies = c.gameObject;

		} 

	}

	public void OnTriggerExit2D(Collider2D c) {

		if ((enemiesLayer.value & 1 << c.gameObject.layer) > 0) {
		//if (c.gameObject.layer == 13) {
			/*
			foreach(GameObject g in detectedEnemies) {
				
				if(g == c.gameObject) {
					detectedEnemies.Remove(g);
					return;
				}
				
			}
			*/

			if(c.gameObject == detectedEnemies)
				detectedEnemies = null;

		} 
		
	}
}
