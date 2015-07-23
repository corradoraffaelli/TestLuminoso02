using UnityEngine;
using System.Collections;

public class ResidualFilesVerifier : MonoBehaviour {

	public string savedLadderFile = "savedLadder";
	public string savedLadderExtention = ".dat";

	public string savedLanternFile = "savedMammut";
	public string savedLanternExtention = ".dat";

	public string savedLevelFile = "savedComingLevel";
	public string savedLevelExtention = ".dat";

	public string savedDoorFile = "savedDoor";
	public string savedDoorExtention = ".dat";

	public bool verifyExistence()
	{
		string path01 = Application.persistentDataPath + "/" + savedLadderFile + savedLadderExtention;

		string path02 = Application.persistentDataPath + "/" + savedLanternFile + savedLanternExtention;

		string path03 = Application.persistentDataPath + "/" + savedLevelFile + savedLevelExtention;

		string path04 = Application.persistentDataPath + "/" + savedDoorFile + "0"+ savedDoorExtention;

		if (System.IO.File.Exists(path01) || System.IO.File.Exists(path02) || System.IO.File.Exists(path03) ||
		    System.IO.File.Exists(path04))
		{
			return true;
		}

		return false;
	}

	public void deleteFiles()
	{
		string path01 = Application.persistentDataPath + "/" + savedLadderFile + savedLadderExtention;
		
		string path02 = Application.persistentDataPath + "/" + savedLanternFile + savedLanternExtention;
		
		string path03 = Application.persistentDataPath + "/" + savedLevelFile + savedLevelExtention;
		
		string path04 = Application.persistentDataPath + "/" + savedDoorFile + "0"+ savedDoorExtention;

		System.IO.File.Delete(path01);
		System.IO.File.Delete(path02);
		System.IO.File.Delete(path03);
		System.IO.File.Delete(path04);
	}
}
