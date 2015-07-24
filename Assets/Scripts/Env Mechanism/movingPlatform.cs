using UnityEngine;
using System.Collections;

public class movingPlatform : MonoBehaviour 
{
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


	//Nell'editor clonare la piattaforma 2 volte e chiamare le due copie destinationSpot e originSpot.
	//Posizionarle nella scena e disattivare la loro sprite.
	//Rendere trigger i loro collider ma lasciarli così da avere un'idea di dove si muoverà la piattaforma.

	void FixedUpdate () 
	{

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

	void Start()
	{
		audioHandler = GetComponent<AudioHandler>();
	}

	void Update()
	{
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
}
