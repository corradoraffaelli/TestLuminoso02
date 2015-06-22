using UnityEngine;
using System.Collections;

public abstract class MessageFSM {

	public abstract GameObject getTarget ();

	public abstract string getInitializationType ();

}

public class PatrolMessageFSM : MessageFSM {

	string initType;
	GameObject target;

	public PatrolMessageFSM(string _initType) {
		initType = _initType;
	}

	public PatrolMessageFSM(string _initType, GameObject _target) {
		initType = _initType;
		target = _target;
	}

	public PatrolMessageFSM(GameObject _target) {
		target = _target;
	}

	public override GameObject getTarget() {
		return target;
	}
	
	public override string getInitializationType() {

		return initType;

	}

}