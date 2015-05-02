using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChaseParameters {

	//Gestione chase-----------------------------------------------------------------------------------
	//Transform chasedTarget;
	//bool avoidingObstacles = false;
	[Range(0.1f,5.0f)]
	public float fTargetFar = 2.0f;
	
	//enemytype nojumpsoftchase
	float offset_MaxDistanceReachable_FromChase = 5.0f;
	bool chaseCharged = false;
	bool chaseCharging = false;
	[Range(0.1f,5.0f)]
	public float tChargingChase = 1.0f;
	float tStartCrash = -1.0f;
	[Range(0.1f,5.0f)]
	public float tToMaxVelocity = 0.5f;
	[Range(0.1f,5.0f)]
	public float tLosingTargerLenght = 5.5f;
	bool losingTarget = false;
	float tStartLosingTarget = -3.0f;
}
