using UnityEngine;
using System.Collections;

interface IStateFSM
{
	void myUpdate();

	void myInitialize();

	void myFinalize();

	void myOnTriggerEnter();

	void myOnCollisionEnter();

	void mySetStates();

	void mySetTransitions();


}