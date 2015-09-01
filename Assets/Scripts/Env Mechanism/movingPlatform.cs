using UnityEngine;
using System.Collections;

public class movingPlatform : MonoBehaviour 
{
	public bool newImplementation = false;

	public Transform destinationSpot;
	public Transform originSpot;
	public float speed;
	public bool Switch = false;

	public bool emitSound = true;
	bool musicSwitchActivated = false;
	bool musicNoSwitchActivated = false;
	AudioHandler audioHandler;
	public string soundNormal = "Direction01";
	public string soundInverse = "Direction02";

	//variabili NEW IMPLEMENTATION
	public float timeToReachDest = 4.0f;
	public bool sameTime = true;
	public float timeToReachOrig = 10.0f;
	
	public bool debugChangingTimes = false;
	
	bool toDest = true;
	
	//float beginToDest;
	//float beginToOrig;
	
	float originTime;
	
	int toDestMultiplier;
	int oldToDestMultiplier = 0;


	//Nell'editor clonare la piattaforma 2 volte e chiamare le due copie destinationSpot e originSpot.
	//Posizionarle nella scena e disattivare la loro sprite.
	//Rendere trigger i loro collider ma lasciarli così da avere un'idea di dove si muoverà la piattaforma.

	void FixedUpdate () 
	{
		if (!newImplementation) {
			if(transform.position == destinationSpot.position)
			{
				Switch = true;
			}
			if(transform.position == originSpot.position)
			{
				Switch = false;
			}
			
			if(Switch)
			{
				transform.position = Vector2.MoveTowards(transform.position, originSpot.position, speed);
			}
			else
			{
				transform.position = Vector2.MoveTowards(transform.position, destinationSpot.position, speed);
			}

		}

	}

	void Start()
	{

		audioHandler = GetComponent<AudioHandler>();

		if (newImplementation) {
			if (originSpot != null && destinationSpot != null) {
				if (sameTime)
					timeToReachOrig = timeToReachDest;
				
				transform.position = originSpot.position;
				
				//beginToDest = Time.time;
				
				originTime = Time.time;
			}
		}
	}

	void Update()
	{
		if (!newImplementation) {
			if (emitSound)
			{
				if (!Switch && !musicNoSwitchActivated && audioHandler != null)
				{
					audioHandler.playClipByName(soundNormal);
					audioHandler.stopClipByName(soundInverse);
				}
				
				if (Switch && !musicSwitchActivated && audioHandler != null)
				{
					audioHandler.playClipByName(soundInverse);
					audioHandler.stopClipByName(soundNormal);
				}
			}
		}

		if (newImplementation) {
			if (emitSound)
			{
				if (toDest && !musicNoSwitchActivated && audioHandler != null)
				{
					audioHandler.playClipByName(soundNormal);
					audioHandler.stopClipByName(soundInverse);
				}
				
				if (!toDest && !musicSwitchActivated && audioHandler != null)
				{
					audioHandler.playClipByName(soundInverse);
					audioHandler.stopClipByName(soundNormal);
				}
			}



			if (originSpot != null && destinationSpot != null) {
				
				//destination
				if (toDest)
				{
					float distance = Vector3.Distance(originSpot.position, destinationSpot.position);
					
					float toDo = 0.0f;
					toDo = (distance * Time.deltaTime) / timeToReachDest;
					
					transform.position = Vector3.MoveTowards(transform.position, destinationSpot.position, toDo);
					
					if (transform.position == destinationSpot.position)
					{
						if (debugChangingTimes)
							Debug.Log (this.gameObject.name + " toOrig "+ Time.time);
						toDest = false;
					}
					
				}
				
				else
				{
					float distance = Vector3.Distance(originSpot.position, destinationSpot.position);
					
					float toDo = 0.0f;
					toDo = (distance * Time.deltaTime) / timeToReachOrig;
					
					//Debug.Log ("toDo "+toDo);
					
					transform.position = Vector3.MoveTowards(transform.position, originSpot.position, toDo);
				}
				
				
				//il cambio di direzione avviene solo se è cambiato il multiplo della divisione tra i tempi
				toDestMultiplier = Mathf.FloorToInt( ((Time.time - originTime) / (timeToReachDest+timeToReachOrig)));
				
				if (toDestMultiplier != oldToDestMultiplier)
				{
					toDest = true;
					if (debugChangingTimes)
						Debug.Log (this.gameObject.name + " toDest "+ Time.time);
				}
				oldToDestMultiplier = toDestMultiplier;
				
			}
		}
	}



	
	
}
