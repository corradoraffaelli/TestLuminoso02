using UnityEngine;
using System.Collections;

public class GeneralFinder : MonoBehaviour {
	
	public static GameObject player;
	public static PlayerMovements playerMovements;
	public static GameObject magicLanternLogicOBJ;
	public static MagicLantern magicLanternLogic;
	public static GraphicLantern magicLanternGraphic;
	public static GlassesManager glassesManager;
	public static GameObject magicLantern;
	public static GameObject controller;
	public static CursorHandler cursorHandler;
	public static GameObject canvasPlayingUI;
	public static PlayingUI playingUI;
	public static PlayingUILateral playingUILateral;
	public static PlayingUIGameOver playingUIGameOver;

	public static UnlockableContentUI unlockableContentUI;

	public static GameObject canvasMenu;
	public static InformativeManager informativeManager;
	public static MenuManager menuManager;
	public static PlayStatusTracker playStatusTracker;

	public static TestInformativeManager testInformativeManager;
	public static GameObject testingController;
	public static HintAnalyzer hintAnalyzer;

	// Use this for initialization
	void Awake () {

		controller = GameObject.FindGameObjectWithTag("Controller");
		if (controller != null)
			cursorHandler = controller.GetComponent<CursorHandler>();

		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
			playerMovements = player.GetComponent<PlayerMovements>();

		magicLanternLogicOBJ = GameObject.FindGameObjectWithTag("MagicLanternLogic");
		if (magicLanternLogicOBJ != null)
		{
			magicLanternLogic = magicLanternLogicOBJ.GetComponent<MagicLantern>();
			magicLanternGraphic = magicLanternLogicOBJ.GetComponent<GraphicLantern>();
			glassesManager = magicLanternLogicOBJ.GetComponent<GlassesManager>();
		}

		magicLantern = GameObject.FindGameObjectWithTag("Lantern");

		canvasPlayingUI = GameObject.FindGameObjectWithTag("CanvasPlayingUI");
		if (canvasPlayingUI != null)
		{
			playingUI = canvasPlayingUI.GetComponent<PlayingUI>();
			playingUILateral = canvasPlayingUI.GetComponent<PlayingUILateral>();
			playingUIGameOver = canvasPlayingUI.GetComponent<PlayingUIGameOver>();
		}

		canvasMenu = UtilFinder._FindGameObjectByTag ("CanvasMenu");

		unlockableContentUI = UtilFinder._GetComponentOfGameObjectWithTag<UnlockableContentUI> ("Controller");
		informativeManager = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");
		menuManager = UtilFinder._GetComponentOfGameObjectWithTag<MenuManager> ("Controller");
		playStatusTracker =  UtilFinder._GetComponentOfGameObjectWithTag<PlayStatusTracker> ("Controller");

		testInformativeManager = UtilFinder._GetComponentOfGameObjectWithTag<TestInformativeManager> ("TestController");
		testingController = GameObject.FindGameObjectWithTag("TestController");
		if (testingController != null)
			hintAnalyzer = testingController.GetComponent<HintAnalyzer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
