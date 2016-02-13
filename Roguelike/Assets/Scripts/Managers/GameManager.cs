using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;                          //Delay between each Player turn.

	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	public BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.

	[HideInInspector]
	public bool playersTurn = true;                         //Boolean to check if it's players turn, hidden in inspector but public.

	private Text levelText;                                 //Text to display current level number.
	private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.

	private int level = 1;                                  //Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.

	private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

	public int playerMaxHungry;
	public int playerHungry;
	public int playerMaxLifePoints;
	public int playerLifePoints;
	public int playerStrPoints;
	public int playerDefPoints;
	public int playerDexPoints;
	public int playerSpdPoints;
	public int playerLucPoints;

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		
		enemies = new List<Enemy>();
		

		boardScript = GetComponent<BoardManager>();
		InitGame();
	}

	void GenerateHero()
	{
		playerHungry = 0;
		playerMaxHungry = 40;
		playerMaxLifePoints = 100;
		playerLifePoints = 100;
        playerStrPoints = 5;
		playerDefPoints = 1;
		playerDexPoints = 1;
		playerSpdPoints = 1;
		playerLucPoints = 1;
	}

	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		level++;
		InitGame();
	}

	//Initializes the game for each level.
	void InitGame()
	{
		doingSetup = true;
		if(level == 1)
		{
			GenerateHero();
		}

		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Floor - " + level;
		levelImage.SetActive(true);

		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();

		boardScript.SetupScene(level);
	}

	//Hides black image used between levels
	void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if (playersTurn || enemiesMoving || doingSetup)
			return;

		//Start moving enemies.
		StartCoroutine(MoveEnemies());
	}

	//Call this to add the passed in Enemy to the List of Enemy objects.
	public void AddEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Add(script);
	}

	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		//Set levelText to display number of levels passed and game over message
		levelText.text = "After floor" + level + ", you died.";

		//Enable black background image gameObject.
		levelImage.SetActive(true);

		//Disable this GameManager.
		enabled = false;
	}

	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;

		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);

		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0)
		{
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++)
		{
			if (enemies[i] != null)
			{
				enemies[i].MoveEnemy();
			} 

			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}