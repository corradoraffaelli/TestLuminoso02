using UnityEngine;
using System.Collections;

public abstract class MessageFSM {

	public abstract GameObject getTarget ();

	public abstract string getInitializationType ();

}

public class BasicMessageFSM : MessageFSM {

	string initType;
	GameObject target;

	public BasicMessageFSM(string _initType) {
		initType = _initType;
	}

	public BasicMessageFSM(string _initType, GameObject _target) {
		initType = _initType;
		target = _target;
	}

	public BasicMessageFSM(GameObject _target) {
		target = _target;
	}

	public override GameObject getTarget() {
		return target;
	}
	
	public override string getInitializationType() {

		return initType;

	}

}
