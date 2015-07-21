﻿using UnityEngine;
using System.Collections;

public class ChangeLevelShortcut : MonoBehaviour {

	public string[] levels;

	void Update () {
		if (levels != null && levels.Length != 0)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0) && levels[0] != null && levels[0] != "")
				Application.LoadLevel(levels[0]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1) && levels[1] != null && levels[1] != "")
				Application.LoadLevel(levels[1]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2) && levels[2] != null && levels[2] != "")
				Application.LoadLevel(levels[2]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3) && levels[3] != null && levels[3] != "")
				Application.LoadLevel(levels[3]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4) && levels[4] != null && levels[4] != "")
				Application.LoadLevel(levels[4]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5) && levels[5] != null && levels[5] != "")
				Application.LoadLevel(levels[5]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6) && levels[6] != null && levels[6] != "")
				Application.LoadLevel(levels[6]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7) && levels[7] != null && levels[7] != "")
				Application.LoadLevel(levels[7]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8) && levels[8] != null && levels[8] != "")
				Application.LoadLevel(levels[8]);
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9) && levels[9] != null && levels[9] != "")
				Application.LoadLevel(levels[9]);
		}
	}
}
