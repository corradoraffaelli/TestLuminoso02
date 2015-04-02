using UnityEngine;
using System.Collections;

public class SimpleAI2D : Pathfinding2D 
{
	public bool DEBUG_TIMERASTAR = false;
    public int SearchPerSecond = 5;
    //public Transform Target;
	public GameObject Target;
	
	public Transform myGroundCheckTransf;

    public bool search = true;
	private bool skipOnce = false;
	private bool skippedOnce = false;
    public float tempDistance = 0F;
	private float thresholdHeightDifference = 1.0f;

	//LAYER DA INSEGUIRE
	public int HideLayer;
	public LayerMask HidingLayerMask;


	void Start () 
    {
        //Make sure that we dont dividde by 0 in our search timer coroutine
        if (SearchPerSecond == 0)
            SearchPerSecond = 1;

		getGroundCheck ();

		normalizeSpatialVariables ();

	}

	private void normalizeSpatialVariables() {

		thresholdHeightDifference = thresholdHeightDifference * Mathf.Abs(transform.localScale.y);

	}

	private void getGroundCheck(){
		
		foreach (Transform child in transform) {
			
			if(child.tag=="GroundCheck" || child.name=="GroundCheck") {
				myGroundCheckTransf = child;
				break;
			}
			
		}
		
		if (myGroundCheckTransf == null)
			Debug.Log ("groundCheck non trovato");
		
	}

	void Update ()
	{
		if (Target != null)
		{
			//save distance so we do not have to call it multiple times
			tempDistance = Vector3.Distance(transform.position, Target.transform.position);
			
			//Check if we are able to search
			if (search == true)
			{
				//Start the time
				StartCoroutine(SearchTimer());

				FindPath(myGroundCheckTransf.position, Target.transform.position);
				
			}

		}
		else
		{
		}
	}

	//non usato
	void Update2 ()
	{
		//Make sure we set a player in the inspector!
		
		if (Target != null)
		{
			//save distance so we do not have to call it multiple times
			tempDistance = Vector3.Distance(transform.position, Target.transform.position);
			
			//Check if we are able to search
			if (search == true)
			{
				//Start the time
				StartCoroutine(SearchTimer());
				
				//Now check the distance to the player, if it is within the distance it will search for a new path
				//{
				
				//TODO: da provare
				//se il layer del target appartiene alla maschera, cioè se facendo un AND bitwise (&) fra il layer
				//che avrà tutti i bit a zero tranne quello identificativo del layer, e la maschera dei layer, se il bit
				//del layer sarà uguale, cioè allo stesso posto, il risultato dell'& sarà > 0, se invece i posti saranno diversi
				//tutti i bit andranno a zero, quindi avremo un valore di zero
				
				//TODO: dovrei togliere questo null, in quanto piloto tutto da basicAI
				//verificare che vada bene toglierlo
				/*
				if( (HidingLayerMask.value & 1<<Target.layer) > 0 ) {
					Debug.Log ("il target si è nascosto, lo metto a null");
					Target = null;
					return;

				}
				*/
				
				// if (layermask.value & 1<<layer) 
				
				//TODO: da ELIMINARE
				/*
				if(Target.layer == HideLayer) {
					Target = null;
					return;
				}
				*/
				
				//versione vecchia
				//FindPath(transform.position, Target.transform.position);
				
				//versione nuova di prova
				//Debug.Log("nuovo path : " + Time.time);
				if (Mathf.Abs (Target.transform.position.y - myGroundCheckTransf.position.y) < thresholdHeightDifference || skippedOnce) {
					FindPath(myGroundCheckTransf.position, Target.transform.position);
					skippedOnce = false;
					if(DEBUG_TIMERASTAR)
						Debug.Log("path updated");
				}
				else {

					skippedOnce = true;
					if(DEBUG_TIMERASTAR)
						Debug.Log("skipped once DONE >>>>>>>>>>>>>>>>>>>>>>>>>");
				}

				
			}
			
			//Make sure that we actually got a path! then call the new movement method
			/*
			if (Path.Count > 0)
            {
                MoveAI();
            }
            */
		}
		else
		{
			//Debug.Log("No player set in the inspector!");
		}
	}

	//non usato
	void Update1 ()
    {
        //Make sure we set a player in the inspector!

        if (Target != null)
        {
            //save distance so we do not have to call it multiple times
			tempDistance = Vector3.Distance(transform.position, Target.transform.position);

            //Check if we are able to search
            if (search == true)
            {
				if (Target != null) {
					if (Mathf.Abs (Target.transform.position.y - myGroundCheckTransf.position.y) > thresholdHeightDifference && !skipOnce) {
						
						if(DEBUG_TIMERASTAR)
							Debug.Log ("skipped once true ----------> ");

						skipOnce = true;
					}
					else {
						if(DEBUG_TIMERASTAR)
							Debug.Log("skipped once false <<<<");

						skipOnce = false;

					}
				}
                //Start the time
                StartCoroutine(SearchTimer());

                //Now check the distance to the player, if it is within the distance it will search for a new path
                //{

				//TODO: da provare
				//se il layer del target appartiene alla maschera, cioè se facendo un AND bitwise (&) fra il layer
				//che avrà tutti i bit a zero tranne quello identificativo del layer, e la maschera dei layer, se il bit
				//del layer sarà uguale, cioè allo stesso posto, il risultato dell'& sarà > 0, se invece i posti saranno diversi
				//tutti i bit andranno a zero, quindi avremo un valore di zero

				//TODO: dovrei togliere questo null, in quanto piloto tutto da basicAI
				//verificare che vada bene toglierlo
				/*
				if( (HidingLayerMask.value & 1<<Target.layer) > 0 ) {
					Debug.Log ("il target si è nascosto, lo metto a null");
					Target = null;
					return;

				}
				*/

				// if (layermask.value & 1<<layer) 

				//TODO: da ELIMINARE
				/*
				if(Target.layer == HideLayer) {
					Target = null;
					return;
				}
				*/

				//versione vecchia
				//FindPath(transform.position, Target.transform.position);

				//versione nuova di prova
				//Debug.Log("nuovo path : " + Time.time);
				if(!skipOnce)
					FindPath(myGroundCheckTransf.position, Target.transform.position);
					if(DEBUG_TIMERASTAR)
						Debug.Log("path updated" + (skipOnce? "vero assurdo" : "falso giusto") );
				else {
					if(DEBUG_TIMERASTAR)
						Debug.Log("skipped once DONE >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
				}

            }

            //Make sure that we actually got a path! then call the new movement method
            /*
			if (Path.Count > 0)
            {
                MoveAI();
            }
            */
        }
        else
        {
            //Debug.Log("No player set in the inspector!");
        }
	}

	IEnumerator SearchTimer()
	{
		//Set search to false for an amount of time, and then true again.
		search = false;
		
		yield return new WaitForSeconds(1 / SearchPerSecond);
		
		search = true;
		
	}

    IEnumerator SearchTimer1()
    {
        //Set search to false for an amount of time, and then true again.
        search = false;

        yield return new WaitForSeconds(1 / SearchPerSecond);

		if (Target != null) {
			if (Mathf.Abs (Target.transform.position.y - myGroundCheckTransf.position.y) > thresholdHeightDifference) {

				if(DEBUG_TIMERASTAR)
					Debug.Log ("Seconda wait searchTimer");

				yield return new WaitForSeconds (1.5f);
			}
		}

		search = true;

    }
	/*
    private void MoveAI()
    {
        //Make sure we are within distance + 1 added so we dont get stuck at exactly the search distance
        if (tempDistance < SearchDistance + 1)
        {       
            //if we get close enough or we are closer then the indexed position, then remove the position from our path list, 
			if (Vector3.Distance(transform.position, Path[0]) < 0.2F || tempDistance < Vector3.Distance(Path[0], Target.transform.position)) 
            {
                Path.RemoveAt(0);
            }   

            if(Path.Count < 1)
                return;

            //First we will create a new vector ignoreing the depth (z-axiz).
            Vector3 ignoreZ = new Vector3(Path[0].x, Path[0].y, transform.position.z);
            
            //now move towards the newly created position

            transform.position = Vector3.MoveTowards(transform.position, ignoreZ, Time.deltaTime * Speed);

        }
    }
    */
}
