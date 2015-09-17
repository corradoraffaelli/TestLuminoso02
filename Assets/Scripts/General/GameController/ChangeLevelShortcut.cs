using UnityEngine;
using System.Collections;

public class ChangeLevelShortcut : MonoBehaviour {

	//public string[] levels;

	bool disableMovements = false;
	bool disableMovementsOLD = false;

	void Start()
	{
		Debug.Log (Application.loadedLevel);
		Debug.Log (Application.levelCount);
	}

	void Update () {
		//if (levels != null && levels.Length != 0)
		//{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0))
			{
				automaticSave();
				StartCoroutine(changeScene(2));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
			{
				automaticSave();
				StartCoroutine(changeScene(3));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
			{
				automaticSave();
				StartCoroutine(changeScene(4));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
			{
				automaticSave();
				StartCoroutine(changeScene(5));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
			{
				automaticSave();
				StartCoroutine(changeScene(6));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
			{
				automaticSave();
				StartCoroutine(changeScene(7));
			}
				
			/*
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6) && levels[6] != null && levels[6] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[6]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7) && levels[7] != null && levels[7] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[7]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8) && levels[8] != null && levels[8] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[8]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9) && levels[9] != null && levels[9] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[9]));
			}	
			*/

			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.X))
			{
				disableMovements = true;
			}	
			else
			{
				disableMovements = false;
			}
			playermovementsHandler();
		//}

		//shortcut uccisione player
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
		{
			GeneralFinder.player.SendMessage("c_instantKill", gameObject.tag);
		}

		//shortcut salvataggio
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
		{
			saveInformativeManager();
		}

		//shortcut salvataggio TestUtils
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
		{
			saveAnanlyzeInfo();
		}

		//shortcut salvataggio Hub
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B))
		{
			saveHubInfos();
		}
	}

	void saveHubInfos()
	{
		if (GeneralFinder.unlockedLevelControl != null)
		{
			GeneralFinder.unlockedLevelControl.saveInfo();
		}
		
		if (GeneralFinder.hubLadderControl != null)
		{
			GeneralFinder.hubLadderControl.saveInfo();
		}
		
		if (GeneralFinder.hubLanternControl != null)
		{
			GeneralFinder.hubLanternControl.saveInfo();
		}
	}

	void saveAnanlyzeInfo()
	{
		GeneralFinder.testingController.GetComponent<ZoneAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<SpikeAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<HintAnalyzer>().saveInfos();
	}

	void saveInformativeManager()
	{
		GeneralFinder.informativeManager.c_saveData();
	}

	void automaticSave()
	{
		bool inHub = false;

		if (2 == Application.loadedLevel)
			inHub = true;

		if (!inHub)
		{
			saveInformativeManager();
			saveAnanlyzeInfo();
		}
		else
		{
			saveHubInfos();
		}

		saveInput();
	}

	IEnumerator changeScene(int levelNumber)
	{
		GeneralFinder.playerMovements.enabled = false;
		yield return new WaitForSeconds(1.0f);

		if (GeneralFinder.levelChangerGeneral != null)
			GeneralFinder.levelChangerGeneral.loadLevel (levelNumber);
		else
			Application.LoadLevel(levelNumber);
	}

	void saveInput()
	{
		if (GeneralFinder.inputKeeper.loadSaveState == InputKeeper.LoadSaveState.Save)
			GeneralFinder.inputKeeper.save();
	}

	void playermovementsHandler()
	{
		if (disableMovements != disableMovementsOLD) {
			//disabilita/abilita i movimenti standard
			GeneralFinder.playerMovements.enabled = !disableMovements;

			//disabilita/abilita i collider del player
			Collider2D[] colliders = GeneralFinder.player.GetComponents<Collider2D>();
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = !disableMovements;
			}

			//disabilita/abilita il rigidbody del player
			Rigidbody2D rigidbody = GeneralFinder.player.GetComponent<Rigidbody2D>();
			rigidbody.isKinematic = disableMovements;
		}

		if (disableMovements) {
			float xInput = Input.GetAxis("Horizontal");
			float yInput = Input.GetAxis("Vertical");

			Vector3 actPos = GeneralFinder.player.transform.position;
			Vector3 newPos = new Vector3(actPos.x + Time.deltaTime*10.0f*xInput, actPos.y + Time.deltaTime*10.0f*yInput, actPos.z);
			GeneralFinder.player.transform.position = newPos;
		}

		disableMovementsOLD = disableMovements;
	}
}
