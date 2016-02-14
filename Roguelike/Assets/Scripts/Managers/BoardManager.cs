using UnityEngine;
using System;
using System.Collections.Generic;      
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.


		//Assignment constructor.
		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}


	public int columns = 8;                                         //Number of columns in our game board.
	public int rows = 8;                                            //Number of rows in our game board.

	public GameObject exit;                                         //Prefab to spawn for exit.

	public Count wallCount = new Count(5, 9);                       //Lower and upper limit for our random number of walls per level.
	public Count foodCount = new Count(1, 4);                       //Lower and upper limit for our random number of food items per level.
	public Count equipmentCount = new Count(0, 3);                  //Lower and upper limit for our random number of equipment items per level.
	public Count weaponsCount = new Count(1, 3);                    //Lower and upper limit for our random number of equipment items per level.
	public Count potionsCount = new Count(1, 3);                    //Lower and upper limit for our random number of equipment items per level.

	public GameObject[] floorTiles;                                 //Array of floor prefabs.
	public GameObject[] wallTiles;                                  //Array of wall prefabs.
	public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
	public GameObject[] bossTiles;                                  //Array of enemy prefabs.
	public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.

	public GameObject[] potionsTiles;

	public GameObject[] consumibleTiles1;							//Array of consumible prefabs.
	public GameObject[] equipmentTiles1;							//Array of equipment prefabs.
	public GameObject[] weaponsTiles1;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles2;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles2;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles2;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles3;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles3;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles3;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles4;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles4;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles4;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles5;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles5;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles5;                              //Array of weapons prefabs.

	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	private List<Vector3> gridPositions = new List<Vector3>();      //A list of possible locations to place tiles.

	public TextAsset roomsText; // The Rooms.xml file
	public XMLReader roomsXMLR;
	public PT_XMLHashList roomsXML;

	//Clears our list gridPositions and prepares it to generate a new board.
	void InitialiseList()
	{
		gridPositions.Clear();

		for (int x = 1; x < columns - 1; x++)
		{
			for (int y = 1; y < rows - 1; y++)
			{
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
	}

	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup()
	{
		boardHolder = new GameObject("Board").transform;

		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for (int x = -1; x < columns + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int y = -1; y < rows + 1; y++)
			{
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent(boardHolder);
			}
		}
	}


	//RandomPosition returns a random position from our list gridPositions.
	Vector3 RandomPosition()
	{
		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
		int randomIndex = Random.Range(0, gridPositions.Count);

		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
		Vector3 randomPosition = gridPositions[randomIndex];

		//Remove the entry at randomIndex from the list so that it can't be re-used.
		gridPositions.RemoveAt(randomIndex);

		//Return the randomly selected Vector3 position.
		return randomPosition;
	}


	//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range(minimum, maximum + 1);

		//Instantiate objects until the randomly chosen limit objectCount is reached
		for (int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition();

			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene(int level)
	{
		//Reset inventory		
		if(level == 1)
		{
			PlayerPrefs.SetString("InventoryContent", "");
			PlayerPrefs.SetString("EquipmentContent", "");
		}

		///*
		if (level == 1)
		{
			int bossNumber = 2;
			ReadXML("" + bossNumber);
		}
		//*/
		/*
		if (level % 5 == 0)
		{
			int bossNumber = level / 5 - 1;
			ReadXML(""+ bossNumber);
		}
		*/
		else
		{
			BoardSetup();
			//Reset our list of gridpositions.
			InitialiseList();

			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

			SetupObjects(level);

			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = (int)Mathf.Log(level, 2f);
			enemyCount = 3;
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

			//Instantiate the exit tile in the upper right hand corner of our game board
			Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}

	private void ReadXML(string roomNumber)
	{
		roomsXMLR = new XMLReader(); // Create a PT_XMLReader
		roomsXMLR.Parse(roomsText.text); // Parse the Rooms.xml file
		roomsXML = roomsXMLR.xml["xml"][0]["room"]; // Pull all the <room>s

		BuildRoom(roomNumber);
	}

	public void BuildRoom(string rNumStr)
	{
		PT_XMLHashtable roomHT = null;
		for (int i = 0; i < roomsXML.Count; i++)
		{
			PT_XMLHashtable ht = roomsXML[i];
			if (ht.att("num") == rNumStr)
			{
				roomHT = ht;
				break;
			}
		}
		if (roomHT == null)
		{
			//Utils.tr("ERROR", "LayoutTiles.BuildRoom()",
			//"Room not found: " + rNumStr);
			return;
		}
		BuildRoom(roomHT);
	}

	// Build a room from an XML <room> entry
	public void BuildRoom(PT_XMLHashtable room)
	{
		// Get the texture names for the floors and walls from <room> attributes
		string floorTexStr = room.att("floor");
		string wallTexStr = room.att("wall");
		int bossNumber = int.Parse(room.att("boss"));
		// Split the room into rows of tiles based on carriage returns in the
		// Rooms.xml file
		string[] roomRows = room.text.Split('\n');
		// Trim tabs from the beginnings of lines. However, we're leaving spaces
		// and underscores to allow for non-rectangular rooms.
		for (int i = 0; i < roomRows.Length; i++)
		{
			roomRows[i] = roomRows[i].Trim('\t');
		}
		
		string type, rawType, tileTexStr;
		int height;
		float maxY = roomRows.Length - 1;

		gridPositions.Clear();

		// These loops scan through each tile of each row of the room
		for (int y = 0; y < roomRows.Length; y++)
		{
			for (int x = 0; x < roomRows[y].Length; x++)
			{
				// Set defaults
				height = 0;
				tileTexStr = floorTexStr;
				// Get the character representing the tile
				type = rawType = roomRows[y][x].ToString();
				switch (rawType)
				{
					case " ": // empty space
					case "_": // empty space
							  // Just skip over empty space
						continue;
					case ".": // default floor
							  // Keep type="."
						break;
					case "|": // default wall
						height = 1;
						break;
					default:
						// Anything else will be interpreted as floor
						type = ".";
						break;
				}
				// Set the texture for floor or wall based on <room> attributes
				GameObject toInstantiate = null;
				Vector3 position = new Vector3(x-1, maxY - y -1, 0f);
                if (type == ".")
				{
					gridPositions.Add(position);
					toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				}
				else if (type == "|")
				{
					toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
				}

				if(rawType == "X")
				{
					GameObject bossTile = bossTiles[bossNumber];
					Instantiate(bossTile, position, Quaternion.identity);
				}

				GameObject instance = Instantiate(toInstantiate, position, Quaternion.identity) as GameObject;
			}
		}
	}

	private void SetupObjects(int level)
	{
		int random = Random.Range(0, 100);
		if(level < 5)
		{
			LayoutObjectAtRandom(consumibleTiles1, foodCount.minimum, foodCount.maximum);
			LayoutObjectAtRandom(equipmentTiles1, equipmentCount.minimum, equipmentCount.maximum);
			LayoutObjectAtRandom(weaponsTiles1, weaponsCount.minimum, weaponsCount.maximum);
		}
		else if(level < 10)
		{
			if(random < 60)
			{
				LayoutObjectAtRandom(consumibleTiles1, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles1, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles1, weaponsCount.minimum, weaponsCount.maximum);
			} 
			else
			{
				LayoutObjectAtRandom(consumibleTiles2, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles2, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles2, weaponsCount.minimum, weaponsCount.maximum);
			}
		}
		else if (level < 15)
		{
			if (random < 40)
			{
				LayoutObjectAtRandom(consumibleTiles1, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles1, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles1, weaponsCount.minimum, weaponsCount.maximum);
			}
			else if (random < 75)
			{
				LayoutObjectAtRandom(consumibleTiles2, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles2, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles2, weaponsCount.minimum, weaponsCount.maximum);
			}
			else
			{
				LayoutObjectAtRandom(consumibleTiles3, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles3, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles3, weaponsCount.minimum, weaponsCount.maximum);
			}
		}
		else if (level < 20)
		{
			if (random < 40)
			{
				LayoutObjectAtRandom(consumibleTiles2, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles2, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles2, weaponsCount.minimum, weaponsCount.maximum);
			}
			else if (random < 75)
			{
				LayoutObjectAtRandom(consumibleTiles3, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles3, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles3, weaponsCount.minimum, weaponsCount.maximum);
			}
			else
			{
				LayoutObjectAtRandom(consumibleTiles4, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles4, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles4, weaponsCount.minimum, weaponsCount.maximum);
			}
		} else
		{
			if (random < 40)
			{
				LayoutObjectAtRandom(consumibleTiles3, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles3, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles3, weaponsCount.minimum, weaponsCount.maximum);
			}
			else if (random < 75)
			{
				LayoutObjectAtRandom(consumibleTiles4, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles4, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles4, weaponsCount.minimum, weaponsCount.maximum);
			}
			else
			{
				LayoutObjectAtRandom(consumibleTiles5, foodCount.minimum, foodCount.maximum);
				LayoutObjectAtRandom(equipmentTiles5, equipmentCount.minimum, equipmentCount.maximum);
				LayoutObjectAtRandom(weaponsTiles5, weaponsCount.minimum, weaponsCount.maximum);
			}
		}

		LayoutObjectAtRandom(potionsTiles, potionsCount.minimum, potionsCount.maximum);
	}
}
