using UnityEngine;
using System.Collections;

public class piccolaprova : MonoBehaviour {

	[SerializeField]
	ciaone c;
}

[System.Serializable]
public class ciaone {

	//[Popup ("Warrior", "Mage", "Archer", "Ninja")]
	[PopupAttribute("Warrior", "Mage", "Archer", "Ninja")]
	public string scegli = "Warrior";
	
}
