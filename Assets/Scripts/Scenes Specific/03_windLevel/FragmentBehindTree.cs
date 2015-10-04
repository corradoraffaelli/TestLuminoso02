using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello del vento. Gestisce la stella nascosta dietro l'albero.
/// </summary>

// Corrado
public class FragmentBehindTree : MonoBehaviour {

	public TreeWindController tree;
	public BoxCollider2D fragment;

	bool windActive = false;
	bool wasWindActive = false;

	void Start () {
		if (fragment != null)
			fragment.enabled = false;
	}

	void Update () {
		if (tree != null && fragment != null)
		{
			windActive = tree.IsWindActive;

			if (windActive && !wasWindActive)
				fragment.enabled = true;
			else if (!windActive && wasWindActive)
				fragment.enabled = false;

			wasWindActive = windActive;
		}
	}
}
