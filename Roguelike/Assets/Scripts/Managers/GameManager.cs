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

	private int level = 0;                                  //Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.

	private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

	[HideInInspector]
	public int playerMaxHungry, 
		playerHungry, 
		playerMaxLifePoints, 
		playerLifePoints, 
		playerStrPoints, 
		playerDefPoints, 
		playerDexPoints, 
		playerSpdPoints, 
		playerLucPoints;

	[HideInInspector]
	public List<float> itemsBonus = new List<float>();

	private List<int> StatsBonus = new List<int>();

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		
		enemies = new List<Enemy>();
		
		boardScript = GetComponent<BoardManager>();
	}

	void GenerateHero()
	{
		ChargeBonuses();
		playerHungry = 0;
		playerMaxHungry = 40 + StatsBonus[0];
		playerMaxLifePoints = 100 + StatsBonus[1];
		playerLifePoints = 100 + StatsBonus[1];
        playerStrPoints = 5 + StatsBonus[2];
		playerDefPoints = 1 + StatsBonus[3];
		playerDexPoints = 1 + StatsBonus[4];
		playerSpdPoints = 1 + StatsBonus[5];
		playerLucPoints = 1 + StatsBonus[6];
	}

	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		Debug.Log("OnLevelWasLoaded"+index);
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

	//Call this to add the passed in Enemy to the List of Enemy objects.
	public void RemoveEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Remove(script);
	}

	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		//Set levelText to display number of levels passed and game over message
		levelText.text = "In the level - " + level + ", you died.";

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
			yield return new WaitForSeconds(enemies[i].moveTime/2);
		}

		playersTurn = true;
		enemiesMoving = false;
	}

	private void ChargeBonuses()
	{
		string content = PlayerPrefs.GetString("StatsSkills");
		if (content != string.Empty)
		{
			string[] splitContent = content.Split(';');
			foreach (string line in splitContent)
			{
				if (!string.IsNullOrEmpty(line))
				{
					string[] splitValues = line.Split('/');
					float bonus = float.Parse(splitValues[2]);
					this.StatsBonus.Add(Mathf.RoundToInt(bonus));
				}
			}
		} else
		{
			for(int i = 0; i<7; i++)
			{
				StatsBonus.Add(0);
			}
		}

		content = PlayerPrefs.GetString("ItemSkills");
		if (content != string.Empty)
		{
			string[] splitContent = content.Split(';');
			foreach (string line in splitContent)
			{
				if (!string.IsNullOrEmpty(line))
				{
					string[] splitValues = line.Split('/');
					float bonus = float.Parse(splitValues[2]);
					this.itemsBonus.Add(bonus);
				}
			}
		} else
		{
			for (int i = 0; i < 7; i++)
			{
				itemsBonus.Add(1f);
			}
		}
	}
}