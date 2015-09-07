//#define _DEBUG

using UnityEngine;
using System.Collections;



public abstract class HEnemyStateFSM : HGenericStateFSM {
	
	#region VARIABLES

	protected PlayerMovements playerScript;
	protected AIParameters par;
	protected StatusParameters statusPar;

	#region QUICKOWNREF

	protected AudioHandler audioHandler {

		get{
			if (par != null) {

				if (par.audioHandler != null) {

					return par.audioHandler;

				}

			}

			return null;
		}

	}

	protected BoxCollider2D weakPointCollider {

		get {
			if(par !=null) {

				if(par.myWeakPointCollider!=null)
					return par.myWeakPointCollider;

			}

			return null;
		}

	}

	protected GameObject weakPoint {
		
		get {
			if(par!=null) {
				if(par.myWeakPoint!=null)
					return par.myWeakPoint;
				
			}
			
			return null;
		}
		
		
	}

	protected bool _instantKill {
		get{ return par.instantKill;}
		set{ par.instantKill = value;}
		
	}

	protected Rigidbody2D _rigidbody {
		get{ return par._rigidbody;}
		set{ par._rigidbody = value;}
		
	}
	
	protected BoxCollider2D _boxCollider {
		get{ return par._boxCollider;}
		set{ par._boxCollider = value;}
		
	}
	
	protected CircleCollider2D _circleCollider {
		get{ return par._circleCollider;}
		set{ par._circleCollider = value;}
		
	}

	#region LAYERS

	protected int defaultLayer {
		get{
			if (par != null) {
				return par.defaultLayer;
				
			}
			return -1;
		}
		set {
			if (par != null) {
				par.defaultLayer = value;
				
			}
			
		}
		
	}
	protected int deadLayer {
		get{
			if (par != null) {
				return par.deadLayer;
				
			}
			return -1;
		}
		set {
			if (par != null) {
				par.deadLayer = value;
				
			}
			
		}
		
	}

	#endregion LAYERS
	
	protected GameObject _target {
		get{ return par._target;}
		set{ par._target = value;}
		
	}
	
	protected GameObject _fleeTarget {
		get{ return par._fleeTarget;}
		set{ par._fleeTarget = value;}
		
	}
	
	#region LAYERMASKS
	
	
	protected LayerMask targetLayers {
		get{ return par.targetLayers;}
		set{ par.targetLayers = value;}
	}
	protected LayerMask fleeLayer{
		get{ return par.fleeLayer;}
		set{ par.fleeLayer = value;}
	}
	protected LayerMask hidingLayer{
		get{ return par.hidingLayer;}
		set{ par.hidingLayer = value;}
	}
	protected LayerMask obstacleLayers{
		get{ return par.obstacleLayers;}
		set{ par.obstacleLayers = value;}
	}
	
	
	#endregion LAYERMASKS
	
	#endregion QUICKOWNREF
	
	#endregion VARIABLES

	//-----

	
	public HEnemyStateFSM(string _stateName, int _stateId, GameObject _gameo, int _hLevel, bool _finalHLevel, HGenericStateFSM _fatherState, AIAgent1 _scriptAIAgent)
		: base(_stateName,_stateId, _gameo, _hLevel, _finalHLevel, _fatherState, _scriptAIAgent){
		
		playerScript = gameObject.GetComponent<PlayerMovements> ();
		
		if (playerScript == null) {
			#if _WARNING_DEBUG
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
			#endif
			
		}
		
		par = gameObject.GetComponent<AIParameters> ();
		
		if (par == null) {
			#if _WARNING_DEBUG
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
			#endif
			
		}
		
		statusPar = par.statusParameters;
		
	}

	public HEnemyStateFSM(string _stateName, GameObject _gameo, bool _finalHLevel, HGenericStateFSM _fatherState, AIAgent1 _scriptAIAgent)
	: base(_stateName, _gameo, _finalHLevel, _fatherState, _scriptAIAgent){
		
		playerScript = gameObject.GetComponent<PlayerMovements> ();
		
		if (playerScript == null) {
			#if _WARNING_DEBUG
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
			#endif
			
		}
		
		par = gameObject.GetComponent<AIParameters> ();
		
		if (par == null) {
			#if _WARNING_DEBUG
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
			#endif
			
		}
		
		statusPar = par.statusParameters;
		
	}
	
	
	#region USEFULMETHODS
	
	protected void i_move(float speed){
		
		//Debug.Log ("mi muovo" + (i_facingRight()? "destra" : "sinistra") );
		//TODO: da cambiare dentro playermovement il nome dei parametri...in particolare maxspeed
		playerScript.c_moveManagement(!i_facingRight(), i_facingRight(), speed);
	}
	
	protected bool i_facingRight() {
		
		return playerScript.FacingRight;
		
	}
	
	protected void i_flip() {
		
		playerScript.c_flip ();
		//Debug.Log ("flippato : " + gameObject);
		
		//TODO: ottimizzare
		foreach (Transform child in transform) {
			
			if(child.name=="StatusImg") {
				
				child.localScale = new Vector3(-child.localScale.x,child.localScale.y, child.localScale.z);
				
				break;
			}
			
		}
		
	}

	protected void i_alert(bool _alert) {

		playerScript.c_alert (_alert);

	}

	protected void i_charged(bool _charged) {
		
		playerScript.c_charged (_charged);
		
	}
	
	protected void i_stunned(bool isStun) {
		
		playerScript.c_stunned (isStun);
		
		if (isStun == false) {
			
			par.myWeakPoint.SendMessage("c_setBouncy", true);
		}
		//gameObject.SendMessage ("c_setBouncy", true);
		
	}
	
	protected void checkFlipNeedForCollision(Collision2D c) {
		
		#if _DEBUG
		Debug.Log ("COLL - entrato in collisione con " + c.gameObject.name);
		#endif
		
		
		if (c.gameObject.tag != "Player") {
			
			//if(!isUnderMyFeet(c) && !isOneWayPlatform(c)) {
			if(!isUnderMyFeet(c) && !isCrossable(c)) {
				#if _DEBUG
				Debug.Log("Collisione! mi flippo!");
				#endif
				//Debug.Log("sono " + gameObject.name + " e mi flippo per " + c.gameObject.name);
				i_flip();
				
			}
			else {
				/*
				if(audioHandler!=null) {
					if(needPlayFallSound(c)) {
						//Debug.Log(gameObject.name + "collision con " + c.gameObject.name); 
						audioHandler.playClipByName("Tonfo");
					}
				}
				*/
			}
		}

	}

	bool isCrossable(Collision2D c) {

		if (c.gameObject.tag == "Crossable") {

			return true;

		}

		return false;
	}

	bool isOneWayPlatform(Collision2D c) {

		PlatformEffector2D pe = c.gameObject.GetComponent<PlatformEffector2D> ();

		if (pe != null) {

			return true;

		}
		else {

			return false;

		}
	}

	/*
	protected bool isUnderMyFeet1(Collision2D c) {
		
		GameObject collisionObj = c.gameObject;
		
		BoxCollider2D bc = collisionObj.GetComponent<BoxCollider2D> ();
		
		ContactPoint2D []contactPoints =  c.contacts;
		
		bool underMyFeet = false;
		//Debug.Log ("check se sotto i piedi");
		//TODO : scorrere tutti i punti?
		foreach (ContactPoint2D cp in contactPoints) {
			
			//Debug.Log ("I'm at : x " + transform.position.x + " y " + transform.position.y + " and the contact point is at : x " + cp.point.x + " y " + cp.point.y);
			
			//TODO: dovrei prendere meglio le misure, in base alla larghezza del player, o meglio, del suo collider

			float radius = _circleCollider.radius;

			float threesh = radius * 0.9f;

			if( Mathf.Abs(cp.point.x - transform.position.x) > threesh) {
				//Debug.Log ("NON SOTTO");
				underMyFeet = false;
				//Debug.Log("NOT UNDER 1" + c.gameObject.name + " n punti " + contactPoints.Length);
			}
			else {

				//Debug.Log ("SOTTO");
				//TODO: dovrei prendere meglio le misure
				if( Mathf.Abs(cp.point.y - transform.position.y) < 0.1f) {

					//Debug.Log("UNDER" + c.gameObject.name + " n punti " + contactPoints.Length);
					underMyFeet = true;
					break;
				}
				else {
					//Debug.Log("NOT UNDER 2" + c.gameObject.name + " n punti " + contactPoints.Length);
				}

			}
		}
		
		//Debug.Log ("fine punti contatto");
		
		
		return underMyFeet;
		
	}
	*/

	protected bool isUnderMyFeet(Collision2D c) {
		
		GameObject collisionObj = c.gameObject;
		
		BoxCollider2D bc = collisionObj.GetComponent<BoxCollider2D> ();
		
		ContactPoint2D []contactPoints =  c.contacts;
		
		bool underMyFeet = false;

		//TODO : scorrere tutti i punti?
		foreach (ContactPoint2D cp in contactPoints) {
			
			//Debug.Log ("I'm at : x " + transform.position.x + " y " + transform.position.y + " and the contact point is at : x " + cp.point.x + " y " + cp.point.y);

			float radius = _circleCollider.radius;

			if( Mathf.Abs( _circleCollider.bounds.center.x - cp.point.x ) > radius * 0.8f && 
			    (_circleCollider.bounds.center.y - cp.point.y) < radius * 0.5f ) {
				//Debug.Log(gameObject.name + " NO SOTTO I MIEI PIEDI! " + c.gameObject.name);

				underMyFeet = false;
				break;

			}
			else {
				//Debug.Log(gameObject.name + " SOTTO I MIEI PIEDI!" + c.gameObject.name);
				underMyFeet = true;
				break;

			}

		}
		
		return underMyFeet;
		
	}



	protected bool needPlayFallSound(Collision2D c) {
		
		GameObject collisionObj = c.gameObject;
		
		BoxCollider2D bc = collisionObj.GetComponent<BoxCollider2D> ();
		
		ContactPoint2D []contactPoints =  c.contacts;

		//TODO : scorrere tutti i punti?
		foreach (ContactPoint2D cp in contactPoints) {

			float radius = _circleCollider.radius;
			
			if( Mathf.Abs( _circleCollider.bounds.center.x - cp.point.x ) < radius * 0.2f ) {
				//Debug.Log("ciao0");
				return true;

			}
			else {
				//Debug.Log("ciao1");
			}
			
		}
		
		return false;
		
	}
	
	protected void moveTowardTarget(GameObject myTarget, float speed) {
		
		
		if (Mathf.Abs (myTarget.transform.position.y - transform.position.y) > 1.0f) {
			//diversa dalla mia altezza
			
			if (Mathf.Abs (myTarget.transform.position.x - transform.position.x) > 1.0f) {
				
				#if _MOVEMENT_DEBUG
				Debug.Log ("TARGET - target più alto di me e distante");
				#endif
				
				i_move (speed * 0.85f);
			} else {
				
				#if _MOVEMENT_DEBUG
				Debug.Log ("TARGET - target più alto di me e sopra di me");
				#endif
				
				i_move (speed * 0.7f);
			}
			
		} 
		else {
			//stessa mia altezza
			
			#if _MOVEMENT_DEBUG
			Debug.Log ("TARGET - target alla mia stessa altezza");
			#endif
			
			moveCorrectVerse (myTarget, speed);
			
		}
	}
	
	protected void moveCorrectVerse(GameObject myTarget, float speed) {
		
		
		if( (myTarget.transform.position.x > transform.position.x ) && !i_facingRight()) {
			i_flip ();
		}
		
		if( (myTarget.transform.position.x < transform.position.x ) && i_facingRight()) {
			i_flip ();
		}
		
		i_move (speed);
		
	}

	protected void checkKillPlayerCollision(Collision2D co) {

		//Debug.Log ("KILL");

		if (co.gameObject.tag == "Player") {
			//co.gameObject.transform.SendMessage ("c_stunned", true);
			
			//GameObject.FindGameObjectWithTag ("Controller").GetComponent<PlayStatusTracker> ().inPlayMode = false;

			if(Mathf.Abs(co.transform.localScale.x)<=Mathf.Abs(transform.localScale.x)) {
				//Debug.Log("AHI ++++++++++++++++++++++" + co.gameObject.name + " " + co.transform.localScale.x + " - " + gameObject.name + " " + transform.localScale.x);
				co.gameObject.transform.SendMessage ("c_instantKill");
				Vector2 dist = co.gameObject.transform.position - transform.position;
				
				Rigidbody2D r = co.gameObject.GetComponent<Rigidbody2D>();
				r.velocity = new Vector2(0.0f, 0.0f);
				r.AddForce(300.0f*dist.normalized);
			}
			else {

				_instantKill = true;

			}

			/*
			if(par.canKillPlayer) {
				co.gameObject.transform.SendMessage ("c_instantKill");
				Vector2 dist = co.gameObject.transform.position - transform.position;
				
				Rigidbody2D r = co.gameObject.GetComponent<Rigidbody2D>();
				r.velocity = new Vector2(0.0f, 0.0f);
				r.AddForce(300.0f*dist.normalized);
			}
			else {

				_instantKill = true;

			}
			*/

			//c_playerStunned(true);
			//return true;
		}
		else {
			
			//return false;
			
		}
	}
	
	protected bool getStatusSpriteRenderer(ref SpriteRenderer _statusSR) {
		
		GameObject statusImgObj = null;
		
		foreach (Transform child in transform) {
			
			if(child.gameObject.name=="StatusImg") {
				statusImgObj = child.gameObject;
			}
			
		}
		
		if (statusImgObj != null) {
			
			_statusSR = statusImgObj.GetComponent<SpriteRenderer>();
			return true;
		}
		
		return false;
		
	}
	
	
	protected void setLayer(int _layer) {
		
		gameObject.layer = _layer;
		
		
	}
	
	
	
	protected void setDefaultLayer() {
		
		gameObject.layer = defaultLayer;
		
	}
	
	protected void setEnemiesStunnedLayer() {
		
		gameObject.layer = LayerMask.NameToLayer("EnemiesStunned");
		
	}
	
	protected void setDeadLayer() {
		
		//Debug.Log("il deadlayer è " + deadLayer.ToString());
		
		//gameObject.layer = deadLayer;
		gameObject.layer = deadLayer;
		
	}
	
	protected bool getRangeOfView(ref float _rov){
		
		GameObject range = null;
		
		foreach (Transform child in transform) {
			if(child.gameObject.name=="RangeOfView") {
				range = child.gameObject;
			}
			
		}
		
		if (range != null) {
			
			if(range.transform.localPosition.x < 0) {
				#if _WARNING_DEBUG
				Debug.Log("ATTENZIONE - L'empty 'RangeOfView' è in una posizione negativa");
				#endif
			}
			_rov = Mathf.Abs( range.transform.localPosition.x ) * Mathf.Abs( transform.localScale.x );
			
			return true;
			
		}
		else {
			#if _WARNING_DEBUG
			Debug.Log("ATTENZIONE - RangeOfView NON trovato");
			#endif
			
			return false;
			
		}
		
	}
	
	protected IEnumerator checkFlipNeed() {

		//Debug.Log ("NEED");

		Vector3 _prevPosition = transform.position;
		
		while (true) {
			
			yield return new WaitForSeconds (1.5f);
			
			Vector3 dist = transform.position - _prevPosition;

			//TODO: valore da verificare se ottimale
			if(dist.magnitude < 0.05f /** transform.localScale.x*/) {
				#if _MOVEMENT_DEBUG
				Debug.Log ("FLIPPED");
				#endif


				i_flip();
			}
			
			_prevPosition = transform.position;
			
		}
	}

	protected bool Any2KScheckInstantKill() {

		return _instantKill;

	}

	
	#endregion USEFULMETHODS
	
}

