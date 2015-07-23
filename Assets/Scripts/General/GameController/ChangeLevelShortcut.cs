using UnityEngine;
using System.Collections;

public class ChangeLevelShortcut : MonoBehaviour {

	public string[] levels;

	void Update () {
		if (levels != null && levels.Length != 0)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0) && levels[0] != null && levels[0] != "")
			{
				save();
				Application.LoadLevel(levels[0]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1) && levels[1] != null && levels[1] != "")
			{
				save();
				Application.LoadLevel(levels[1]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2) && levels[2] != null && levels[2] != "")
			{
				save();
				Application.LoadLevel(levels[2]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3) && levels[3] != null && levels[3] != "")
			{
				save();
				Application.LoadLevel(levels[3]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4) && levels[4] != null && levels[4] != "")
			{
				save();
				Application.LoadLevel(levels[4]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5) && levels[5] != null && levels[5] != "")
			{
				save();
				Application.LoadLevel(levels[5]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6) && levels[6] != null && levels[6] != "")
			{
				save();
				Application.LoadLevel(levels[6]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7) && levels[7] != null && levels[7] != "")
			{
				save();
				Application.LoadLevel(levels[7]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8) && levels[8] != null && levels[8] != "")
			{
				save();
				Application.LoadLevel(levels[8]);
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9) && levels[9] != null && levels[9] != "")
			{
				save();
				Application.LoadLevel(levels[9]);
			}	
		}

		//shortcut uccisione player
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
		{
			GeneralFinder.player.SendMessage("c_instantKill");
		}

		//shortcut salvataggio
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
		{
			GeneralFinder.informativeManager.c_saveData();
		}
	}

	void save()
	{
		if (levels != null && levels.Length != 0 && levels[0] == Application.loadedLevelName)
			return;
		else
			//salva
			Debug.Log ("dovrebbe esserci il salvataggio nel cambio scena");
	}
}
