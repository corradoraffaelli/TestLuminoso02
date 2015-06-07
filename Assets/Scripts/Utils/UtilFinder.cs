using UnityEngine;
using System.Collections;

public class UtilFinder {
	
	//GAMEOBJECT FINDER-------------------------------------------------------------------------------------------------------------------------
	
	public static GameObject _FindGameObjectByTag(string _tag) {
		
		GameObject ob = null;
		
		ob = GameObject.FindGameObjectWithTag (_tag);
		
		if (ob == null) {
			
			Debug.Log ("_WARNING_ - NOT FOUND gameobject with tag '" + _tag + "' ");
			
		}
		
		return ob;
		
	}
	
	public static GameObject _FindGameObjectByName(string _name) {
		
		GameObject ob = null;
		
		ob = GameObject.FindGameObjectWithTag (_name);
		
		if (ob == null) {
			
			Debug.Log ("_WARNING_ - NOT FOUND gameobject with name '" + _name + "' ");
			
		}
		
		return ob;
		
	}
	
	public static GameObject[] _FindChildrenByTag(string tag, GameObject father = null) {
		
		ArrayList children = new ArrayList ();
		
		if (father == null) {
			//search into the scene
			
			
			
			GameObject []childr = GameObject.FindGameObjectsWithTag(tag);
			
			if(childr != null) {
				
				return childr;
				
			}
			else {
				
				//if end if
				
				Debug.Log("_WARNING_ - NOT FOUND gameobjects children with tag '" + tag + "'  inside the scene");
				
				return null;
				
			}
		}
		else {
			//search into a specific gameobject
			
			if (recursiveMultipleSearchByTag (father, tag, ref children)) {
				
				GameObject []childrenArr = new GameObject[children.Count];
				
				childrenArr = (GameObject[]) children.ToArray(typeof(GameObject));
				
				return childrenArr;
			}
			else {
				//if end if
				
				Debug.Log("_WARNING_ - NOT FOUND gameobjects children with tag '" + tag + "'  inside the gameobject '" + father.name + "'");
				
				return null;
			}
		}
		
	}
	
	static bool recursiveMultipleSearchByTag(GameObject father, string _tag, ref ArrayList _outputChildren) {
		
		foreach (Transform child in father.transform) {
			
			if(child.tag==_tag) {
				
				_outputChildren.Add(child.gameObject);
				
			}
			
			if(child.childCount>0)
				recursiveMultipleSearchByTag(child.gameObject, _tag, ref _outputChildren);
			
			
		}
		
		if (_outputChildren.Count > 0)
			return true;
		else
			return false;
		
		
	}
	
	public static GameObject _FindChildByTag(string tag, GameObject father = null ) {
		
		GameObject child = null;
		
		if (father == null) {
			//search into the scene
			
			child = GameObject.FindGameObjectWithTag(tag);
			
			if(child != null) {
				
				return child;
				
			}
			else {
				
				//if end if
				
				Debug.Log("_WARNING_ - NOT FOUND gameobject with tag '" + tag + "'  inside the scene");
				
				return null;
				
			}
		}
		else {
			//search into a specific gameobject
			
			if (recursiveSearchByTag (father, tag, ref child)) {
				return child;
			}
			else {
				//if end if
				
				Debug.Log("_WARNING_ - NOT FOUND gameobject child with tag '" + tag + "' inside the gameobject '" + father.name + "'");
				
				return null;
			}
		}
	}
	
	static bool recursiveSearchByTag(GameObject father, string _tag, ref GameObject _outputChild) {
		
		foreach (Transform child in father.transform) {
			
			if(child.tag==_tag) {
				
				_outputChild = child.gameObject;
				return true;
				
			}
			else {
				
				if(recursiveSearchByTag(child.gameObject, _tag, ref _outputChild)) {
					
					return true;
					
				}
				else {
					
					//niente?
					
				}
				
				
			}
			
			
		}
		
		return false;
		
	}
	
	public static GameObject _FindChildByName(string name, GameObject father = null ) {
		
		GameObject child = null;
		
		if (recursiveSearchByName (father, name, ref child)) {
			return child;
		}
		else {
			//if end if
			
			Debug.Log("_WARNING_ - NOT FOUND gameobject child with name '" + name + "' inside the gameobject '" + father.name + "'");
			
			return null;
		}
		
	}
	
	static bool recursiveSearchByName(GameObject father, string _name, ref GameObject _outputChild) {
		
		foreach (Transform child in father.transform) {
			
			if(child.name==_name) {
				
				_outputChild = child.gameObject;
				return true;
				
			}
			else {
				
				if(recursiveSearchByName(child.gameObject, _name, ref _outputChild)) {
					
					return true;
					
				}
				else {
					
					//niente?
					
				}
				
				
			}
			
			
		}
		
		return false;
		
	}
	
	//COMPONENT FINDER-------------------------------------------------------------------------------------------------------------------------
	
	//da migliorare
	static public T _GetComponent <T> (GameObject _gameObject) {
		
		T comp = _gameObject.GetComponent<T> ();
		
		if (comp.ToString() == "null") {
			
			Debug.Log ("_WARNING_ - NOT FOUND component '" + typeof(T) + "' inside '" + _gameObject.name +  "'");
			
		}
		
		return comp;
		
	}
	
	static public T _GetComponentOfGameObjectWithTag <T> (string _tag) {
		
		GameObject ob = UtilFinder._FindGameObjectByTag (_tag);
		
		if (ob == null) {
			//Debug.Log ("_WARNING_ - NOT FOUND gameobject with tag '" + _tag + "' inside the scene, during getting its component '" + typeof(T) + "'");
			return default(T);
		}
		else {
			
			T comp = ob.GetComponent<T> ();
			
			if (comp.ToString () == "null") {
				
				Debug.Log ("_WARNING_ - NOT FOUND component '" + typeof(T) + "' inside '" + ob.name + "'");
				
			}
			
			return comp;
		}
	}
	
	static public T _GetComponentOfChildrenWithTag <T> (GameObject _gameObject, string _tag) {
		
		GameObject ob = UtilFinder._FindChildByTag (_tag, _gameObject);
		
		if (ob != null) {
			T comp = ob.GetComponent<T> ();
			
			if (comp.ToString () == "null") {
				
				Debug.Log ("_WARNING_ - NOT FOUND component '" + typeof(T) + "' inside '" + ob.name + "'");
				
			}
			
			return comp;
			
		}
		else {
			Debug.Log ("_WARNING_ - NOT FOUND gameobject child with tag '" + _tag + "' inside the gameobejct '" + _gameObject.name + "', during getting its component '" + typeof(T) + "'");
			//T comp = null;
			return default(T);
		}
	}
	
}
