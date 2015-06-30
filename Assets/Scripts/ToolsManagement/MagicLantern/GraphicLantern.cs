using UnityEngine;
using System.Collections;

public class GraphicLantern : MonoBehaviour {

	GameObject raggio_cerchio;
	GameObject raggio;
	GameObject cameraPoint;
	GameObject camera;
	GameObject sostegno;
	GameObject projectionObject;
	SpriteRenderer spriteRendererFakeProjection;

	GameObject GroundCheckUpperLeft;
	GameObject GroundCheckBottomRight;
	public LayerMask GroundLayers;

	public Vector2 positionOnGround = new Vector2(0.25f, 0.62f);
	public Vector2 positionOnPlayer = new Vector2(-0.25f, 0.35f);
	public float zPositionEnvironment = 0.0f;
	public float resizeFactor = 4.0f;
	public float maxProjectingDistance = 8.5f;
	public float minProjectingDistance = 1.0f;
	public bool newSizeImplementation = true;
	public float defaultCircleSize = 3.5f;
	public float defaultDircleDistance = 4.0f;
	[Range(0.0f,3.0f)]
	public float changeSizeMultiplier = 1.8f;
	public bool limitProjectionDistance = true;
	[Range(0.0f,1.0f)]
	public float alphaProjection = 0.5f;

	public Sprite normalRay;
	public Sprite normalCircle;
	public Sprite pressedRay;
	public Sprite pressedCircle;
	public Sprite badRay;
	public Sprite badCircle;

	Sprite goodGlassSprite;
	Sprite badGlassSprite;
	Sprite projectionSprite;
	Sprite blurredSprite;
	Sprite emptySprite;

	Vector3 cameraPointPos;
	Vector3 _direction;

	float xSize;
	float ySize;
	Bounds boundsRay;
	
	SpriteRenderer spRendRay;
	SpriteRenderer spRendCircle;

	GameObject lantern;
	GameObject player;
	GameObject controller;
	CursorHandler cursorHandler;
	GlassesManager glassesManager;

	public GameObject clouds;

	//3 variabili per ora inutili, pensate nel caso debbano essere presenti muri su cui poter proiettare
	float lastTimeUpdate = 0.0f;
	float timeToUpdate = 1.3f;
	public GameObject[] walls;

	bool touchingBadWall = false;

	public float standardFakeProjectionRotation = 0.0f;

	bool lanternOnPlayerPosition;
	public bool LanternOnPlayerPosition{
		get{return lanternOnPlayerPosition;}
	}

	public void switchOffRay()
	{
		raggio_cerchio.SetActive (false);
		raggio.SetActive (false);
		projectionObject.SetActive (false);
	}

	void switchOnRay()
	{
		raggio_cerchio.SetActive (true);
		raggio.SetActive (true);
		projectionObject.SetActive (true);
	}

	public void switchOnLantern()
	{
		lantern.SetActive (true);
		switchOnRay ();
		switchSupport();
	}

	public void switchOffLantern()
	{
		lantern.SetActive (false);
	}

	public void leaveLantern()
	{
		lantern.transform.parent = null;
	}

	public void switchSupport(bool enable = true)
	{
		sostegno.SetActive (enable);
	}

	public void setSuperSortingLayer(bool enable = true)
	{
		string sortingLayerName;
		if (enable)
			sortingLayerName = "LanternSuper";
		else
			sortingLayerName = "Lantern";
		camera.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
		sostegno.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
	}

	public void takeLantern()
	{
		lantern.transform.parent = player.transform;
		//lantern.transform.localPosition = new Vector3(0.4f,0.8f,0.0f);
		//lantern.transform.localPosition = new Vector3(-0.25f,0.62f,0.0f);
		lantern.transform.localPosition = new Vector3(positionOnPlayer.x,positionOnPlayer.y,0.0f);
		Vector3 actualScale = lantern.transform.localScale;
		lantern.transform.localScale = new Vector3 (Mathf.Abs(actualScale.x),Mathf.Abs(actualScale.y),Mathf.Abs(actualScale.z));

		lanternOnPlayerPosition = false;
	}

	public void setNormalFakeProjectionSprite()
	{
		if (spriteRendererFakeProjection.sprite != projectionSprite) {
			spriteRendererFakeProjection.sprite = projectionSprite;
		}
		if (spriteRendererFakeProjection.color.a != alphaProjection) {
			Color oldColor = spriteRendererFakeProjection.color;
			spriteRendererFakeProjection.color = new Color(oldColor.r, oldColor.g, oldColor.b, alphaProjection);
		}
	}

	public void changeAlphaFakeProjectionSprite(float newAlpha)
	{
		Color oldColor = spriteRendererFakeProjection.color;
		spriteRendererFakeProjection.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
	}

	public Transform getCircleRay()
	{
		return raggio_cerchio.transform;
	}

	public Bounds getProjectionBounds()
	{
		return spriteRendererFakeProjection.bounds;
	}

	public void setLeftLanternRaySrites(bool left = true)
	{
		if (left) {
			spRendRay.sprite = pressedRay;
			spRendCircle.sprite = pressedCircle;
		} else {
			spRendRay.sprite = normalRay;
			spRendCircle.sprite = normalCircle;
		}
	}

	public void setBadRaySprites(bool bad = true)
	{
		if (bad) {
			spRendRay.sprite = badRay;
			spRendCircle.sprite = badCircle;
		} else {
			spRendRay.sprite = normalRay;
			spRendCircle.sprite = normalCircle;
		}
	}


	public bool groundCheck()
	{
		if (GroundCheckBottomRight != null)
			return Physics2D.OverlapArea (new Vector2 (GroundCheckUpperLeft.transform.position.x, GroundCheckUpperLeft.transform.position.y), 
		                              new Vector2 (GroundCheckBottomRight.transform.position.x, GroundCheckBottomRight.transform.position.y), GroundLayers);
		else
			return true;
	}

	public void addRigidbody()
	{
		Rigidbody2D RB = lantern.AddComponent<Rigidbody2D>() as Rigidbody2D;
		//RB.mass = 0.5f;
		RB.gravityScale = 0.5f;
		/*
		Rigidbody2D RB = new Rigidbody2D ();
		lantern.AddComponent<Rigidbody2D> (RB);
		>*/
	}

	public void removeRigidbody()
	{
		Rigidbody2D RB = lantern.GetComponent<Rigidbody2D> ();
		Destroy (RB);
	}

	//da chiamare dopo che la lanterna non è più figlia del player, così che i parametri di posizione siano globali
	public void putLanternOnPlayer()
	{
		//salvo posizioni salienti prima di cambiarle
		Vector3 circlePosition = raggio_cerchio.transform.position;
		Vector3 playerPosition = player.transform.position;

		//setto la posizione della lanterna
		//di default diffX e diffY dovrebbero essere 0, invece si devono fare dei piccoli aggiustamenti...
		//float diffX = 0.0f;
		//float diffY = 0.62f;
		//float diffX = 0.25f;
		//float diffY = 0.62f;
		float diffX = positionOnGround.x;
		float diffY = positionOnGround.y;
		if (lantern.transform.localScale.x < 0.0f)
			lantern.transform.position = new Vector3(playerPosition.x + diffX, playerPosition.y + diffY,playerPosition.z);
		else
			lantern.transform.position = new Vector3(playerPosition.x - diffX, playerPosition.y + diffY,playerPosition.z);

		//risetto la posizione del cerchio
		raggio_cerchio.transform.position = circlePosition;

		//prendo la posizione del punto frontale della sprite della camera, e ci piazzo l'origine del raggio
		cameraPointPos = cameraPoint.transform.position;
		raggio.transform.position = cameraPointPos;

		//prendo la direzione tra l'inizio del raggio e la posizione del cerchio
		_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;

		//cambio la scala del raggio in base alla distanza tra camera e cerchio
		float distance = Vector3.Distance (raggio_cerchio.transform.position, cameraPointPos);
		
		//calcolo della scala
		float actualSize = raggio_cerchio.transform.localScale.x;
		raggio.transform.localScale = new Vector3(distance / xSize,actualSize,1);
		//raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);

		if (lantern.transform.localScale.x<0.0f)
			_direction = new Vector3 (-_direction.x, _direction.y,_direction.z);

		raggio.transform.right = _direction;

		if (!(raggio.transform.localEulerAngles.z > 179 && raggio.transform.localEulerAngles.z < 220)) {
			if (raggio.transform.localEulerAngles.z > 180)
				camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, (raggio.transform.localEulerAngles.z-360) / 2);
			else
				camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.z / 2);
		}

		lanternOnPlayerPosition = true;
	}

	void Awake()
	{
		takeObjectsFromScene ();
	}


	void Start () {

		player = GameObject.FindGameObjectWithTag ("Player");
		controller = GameObject.FindGameObjectWithTag ("Controller");
		cursorHandler = controller.GetComponent<CursorHandler> ();

		if (lantern != null) {
			lantern.SetActive (false);
			takeComponentsFromObjects ();

			//valori dei bounds di default del raggio
			boundsRay = spRendRay.bounds;
			xSize = boundsRay.size.x;
			ySize = boundsRay.size.y;
		}



	}

	/*
	void Update()
	{
		updateWalls ();
	}
	*/

	//update delle variabili locali in relazione al vetrino attuale, gestito dal glassedManager
	public void glassSpriteUpdate()
	{
		if (glassesManager.getActualGlass() != null) {
			goodGlassSprite = glassesManager.getActualGlass().goodProjection;
			badGlassSprite = glassesManager.getActualGlass().badProjection;
			projectionSprite = glassesManager.getActualGlass().spriteObject;
			blurredSprite = glassesManager.getActualGlass().blurredProjection;
			emptySprite = glassesManager.getActualGlass().emptySprite;
		}
	}

	public Vector3 getLanternPosition()
	{
		return lantern.transform.position;
	}

	//movimenti del raggio e della proiezione in relazione alla posizione del player e del cursore
	public void normalMovementsUnderMouse(){
		//primo if per verificare che il cursore non sia troppo vicino alla lanterna, per evitare strani flipping
		//si potrebbe pensare di limitare il cursore, ma per ora troppo complesso
		if (!verifyIfCursorCameraTooNear ()) {
			
			//posiziono la lenterna di fronte al personaggio (ora non serve, la lanterna è "child" del player, da verificare la correttezza della cosa
			//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);

			//serve nel caso ci sia un vetrino sensibile alla rotazione
			//raggio_cerchio.transform.right = new Vector3(1.0f,0.0f,0.0f);

			//posiziono l'origine del cerchio sotto il mouse
			if (limitProjectionDistance && verifyIfTooFar ())
				raggio_cerchio.transform.position = getPositionAlongDirection (cameraPoint.transform.position, cursorHandler.getCursorWorldPosition (), maxProjectingDistance);
			else if (limitProjectionDistance && verifyIfTooNear ())
				raggio_cerchio.transform.position = getPositionAlongDirection (cameraPoint.transform.position, cursorHandler.getCursorWorldPosition (), minProjectingDistance);
			else
				raggio_cerchio.transform.position = cursorHandler.getCursorWorldPosition ();
			
			//prendo la posizione del punto frontale della sprite della camera, e ci piazzo l'origine del raggio
			cameraPointPos = cameraPoint.transform.position;
			raggio.transform.position = cameraPointPos;
			
			//prendo la direzione tra l'inizio del raggio e la posizione del cerchio
			_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;
			
			//cambio la scala del raggio in base alla distanza tra camera e cerchio
			float distance = Vector3.Distance (raggio_cerchio.transform.position, cameraPointPos);
			
			//calcolo della scala
			if (newSizeImplementation)
			{
				float actualSize = defaultCircleSize + (defaultCircleSize*(distance-defaultDircleDistance)*changeSizeMultiplier/10.0f);
				raggio.transform.localScale = new Vector3(distance / xSize,actualSize,1);
				raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);
			}else{
				raggio.transform.localScale = new Vector3(distance / xSize,distance / (ySize*resizeFactor),1);
				raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);
			}
			

			//setto la direzione del raggio
			//se il personaggio non sta guardando verso destra la sua scala è -1, devo perciò correggere quella della direzione, di conseguenza
			PlayerMovements PM = player.GetComponent<PlayerMovements> ();
			if (!PM.FacingRight) {
				_direction = new Vector3 (-_direction.x, _direction.y,_direction.z);
				//il cerchio e la proiezione contenuta in esso non devono flippare insieme al personaggio
				raggio_cerchio.transform.localScale = new Vector3(-raggio_cerchio.transform.localScale.x,raggio_cerchio.transform.localScale.y,raggio_cerchio.transform.localScale.z);
			}
			raggio.transform.right = _direction;
			
			//Debug.Log (raggio.transform.localEulerAngles.z);
			
			//setto la direzione della camera
			if (!(raggio.transform.localEulerAngles.z > 179 && raggio.transform.localEulerAngles.z < 220)) {
				if (raggio.transform.localEulerAngles.z > 180)
					camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, (raggio.transform.localEulerAngles.z-360) / 2);
				else
					camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.z / 2);
			}


			//------------NUOVO------------
			if (!glassesManager.getActualGlass().canBeModified && !glassesManager.getActualGlass().rotateWithLantern)
				standardFakeProjectionRotation = 0.0f;
			projectionObject.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, standardFakeProjectionRotation);

			if (glassesManager.getActualGlass().rotateWithLantern)
			{
				_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;

				projectionObject.transform.right = _direction;

				standardFakeProjectionRotation = projectionObject.transform.localEulerAngles.z;
			}
		}
	}



	public Vector3 getTempProjectionScale()
	{
		return projectionObject.transform.localScale;
	}

	public void rotateTempProjectionByAngle(float inputAngle)
	{
		/*
		_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;
		PlayerMovements PM = player.GetComponent<PlayerMovements> ();
		if (!PM.FacingRight) {
			_direction = new Vector3 (-_direction.x, _direction.y,_direction.z);
			//raggio_cerchio.transform.localEulerAngles = new Vector3(0.0f,0.0f,raggio_cerchio.transform.localEulerAngles.z + 180);
			raggio_cerchio.transform.localScale = new Vector3(-raggio_cerchio.transform.localScale.x, raggio_cerchio.transform.localScale.y, raggio_cerchio.transform.localScale.z);
		}
		
		raggio_cerchio.transform.right = _direction;



		PlayerMovements PM = player.GetComponent<PlayerMovements> ();
		if (raggio_cerchio.transform.localScale.x < 0.0f && inputAngle != 0.0f && inputAngle != 180.0f) {
			inputAngle = inputAngle +180.0f;
			Debug.Log ("I'm in");
		}
		raggio_cerchio.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, inputAngle);
		*/
		//projectionObject.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, inputAngle);

		standardFakeProjectionRotation = inputAngle;
	}

	public void resetCircleAngle()
	{
		raggio_cerchio.transform.right = new Vector3(1.0f,0.0f,0.0f);

	}

	public void resetTempProjectionAngle()
	{
		projectionObject.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
	}

	public Vector3 getTempProjectionAngle()
	{
		return projectionObject.transform.localEulerAngles;
	}

	public float getStandardFakeProjectionRotation()
	{
		return standardFakeProjectionRotation;
	}

	public bool verifyIfInsideWall()
	{
		return false;
	}

	public void instantiateClouds(Vector3 instantiatePosition)
	{
		if (clouds != null)
		{
			if (instantiatePosition == null)
				instantiatePosition = raggio_cerchio.transform.position;
			GameObject instClouds = Instantiate(clouds, instantiatePosition, Quaternion.identity) as GameObject;
			ParticleEmitter particleEmitter = instClouds.GetComponent<ParticleEmitter>();
			particleEmitter.emit = true;
		}
	}

	void updateWalls()
	{
		if (Mathf.Abs (Time.time - lastTimeUpdate) > timeToUpdate) {
			lastTimeUpdate = Time.time;
			walls = GameObject.FindGameObjectsWithTag("ProjectionWall");
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "ProjectionWall") {
			touchingBadWall = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{

	}

	bool verifyIfCursorCameraTooNear()
	{
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;
		
		Vector2 pos_cursor2 = new Vector2 (pos_cursor.x, pos_cursor.y);
		Vector2 pos_player2 = new Vector2 (pos_player.x, pos_player.y);
		Vector2 direction2 = (pos_player2 - pos_cursor2);

		float distance = Vector3.Distance (pos_player, pos_cursor);

		if (Mathf.Abs (distance) < 0.3f)
			return true;
		else
			return false;
	}
	
	//funzione per aggiornare le variabili contenenti le sprites
	//----!!!!! il cambiamento di queste variabili non modifica ciò che viene mostrato a schermo, a meno di un aggiornamento


	Vector3 getPositionAlongDirection(Vector3 startPosition, Vector3 endingPosition, float distance)
	{
		float catetoX = Mathf.Abs (startPosition.x - endingPosition.x);
		float catetoY = Mathf.Abs (startPosition.y - endingPosition.y);
		float ipotenusa = Mathf.Sqrt(catetoX * catetoX + catetoY * catetoY);
		float dX = distance * catetoX / ipotenusa;
		float dY = distance * catetoY / ipotenusa;
		
		float posX, posY;
		if (startPosition.x < endingPosition.x)
			posX = dX + startPosition.x;
		else
			posX = startPosition.x - dX;
		
		if (startPosition.y < endingPosition.y)
			posY = dY + startPosition.y;
		else
			posY = startPosition.y - dY;
		
		Vector3 pos = new Vector3 (posX, posY, zPositionEnvironment);
		return pos;
	}

	bool verifyIfTooFar()
	{
		//Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;
		//Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		if (Mathf.Abs (distance) > maxProjectingDistance)
			return true;
		else
			return false;
	}
	
	//verifica se la proiezione è troppo vicina al player
	bool verifyIfTooNear()
	{
		//Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;
		//Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		if (Mathf.Abs (distance) < minProjectingDistance)
			return true;
		else
			return false;
	}

	void takeObjectsFromScene()
	{
		/*
		foreach (Transform child in player.transform) {
			if (child.name == "MagicLantern") {
				lantern = child.gameObject;
				break;
			}
		}
		*/
		lantern = GameObject.FindGameObjectWithTag ("Lantern");

		if (lantern != null) {
			foreach (Transform child in lantern.transform) {
				if (child.name == "Proiettore")
				{
					camera = child.gameObject;
					foreach (Transform subChild in child.transform) {
						if (subChild.name == "Proiettore_punta")
						{
							cameraPoint = subChild.gameObject;
						}
					}
				}else if(child.name == "raggio"){
					raggio = child.gameObject;
				}else if(child.name == "Sostegno"){
					sostegno = child.gameObject;
				}else if (child.name == "raggio_cerchio"){
					raggio_cerchio = child.gameObject;
					foreach (Transform subChild in child.transform) {
						if (subChild.name == "ProjectedObject")
						{
							projectionObject = subChild.gameObject;
						}
					}
				}else if(child.name == "GroundCheckBR") {
					GroundCheckBottomRight = child.gameObject;
				}
				else if(child.name == "GroundCheckUL") {
					GroundCheckUpperLeft = child.gameObject;
				}
			}
		}


	}

	void takeComponentsFromObjects()
	{
		//componenti Sprite Renderer del raggio
		spRendRay = raggio.GetComponent<SpriteRenderer> ();
		spRendCircle = raggio_cerchio.GetComponent<SpriteRenderer> ();
		
		glassesManager = transform.GetComponent<GlassesManager> ();

		spriteRendererFakeProjection = projectionObject.GetComponent<SpriteRenderer> ();
		//returnParticles = GetComponentInChildren<ReturnParticles> ();
	}

}
