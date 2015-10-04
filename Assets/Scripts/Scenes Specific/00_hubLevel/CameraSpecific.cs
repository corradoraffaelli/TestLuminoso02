using UnityEngine;
using System.Collections;

/// <summary>
/// Classe specifica dell'hub. Fissa la camera al centro, alla giusta distanza.
/// </summary>

// Corrado
public class CameraSpecific : MonoBehaviour {

	public float lateralView = 8.3f;

	void Start () {
		float aspectRatio = Camera.main.aspect;
		float size = lateralView / aspectRatio;
		//Debug.Log ("sizeCamera" + size);
		Camera.main.orthographicSize = size;
	}
}
