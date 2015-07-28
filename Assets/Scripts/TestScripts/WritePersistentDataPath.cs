using UnityEngine;
using System.Collections;

public class WritePersistentDataPath : MonoBehaviour {



	void Start () {
		System.IO.File.WriteAllText("fileFico.txt", Application.persistentDataPath);
	}

	void Update () {
	
	}
}
