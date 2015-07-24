using UnityEngine;
using System.Collections;

public class SetSortingLayerLegacy : MonoBehaviour {

	public string sortingLayerName = "Proiezioni";
	public int sortingLayerInt = 0;

	void Start () {

		ParticleRenderer partRend = GetComponent<ParticleRenderer>();

		if (partRend != null)
		{
			partRend.sortingLayerName = sortingLayerName;
			partRend.sortingOrder = sortingLayerInt;
		}
	}
}
