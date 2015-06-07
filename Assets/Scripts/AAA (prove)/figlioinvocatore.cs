using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class figlioinvocatore : provainvoco {

	// Use this for initialization
	void Start () {

		Component []componenti = GetComponents(typeof(provainvoco));
		
		foreach (Component co in componenti) {
			
			Type tipo = co.GetType ();
			Debug.Log ("stampo il tipo " + tipo.ToString());
			
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected override void CiaoStampa() {
		
		Debug.Log ("ciao - derivato");
		
	}
	
	protected override void MammaStampa() {
		
		Debug.Log ("mamma - derivato");
		
	}

	protected void minkiasi(){

		Debug.Log ("minkiasi - derivato");

	}
}
