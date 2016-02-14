using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject, Destuctible
{
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private int hungry;
	private int maxHungry;
	private int totalMaxLife;
	private int totalStr;
	private int totalDef;
	private int totalDex;
	private int totalSpd;
	private int totalLuc;

	public RectTransform healthTransform;
	private float cachedY;
	private float minXValue;
	private float maxXValue;
	public Text healthText;
	public Image visualHealth;

	public Image hungryIcon;

	private Animator animator;                  //Used to store a reference to the Player's animator component.

	//Start overrides the Start function of MovingObject
	protected override void Start()
	{
		animator = GetComponent<Animator>();

		cachedY = healthTransform.position.y;
		maxXValue = healthTransform.position.x;
		minXValue = healthTransform.position.x - healthTransform.rect.width;

		//Get the current stats stored in GameManager.instance between levels.
		maxHungry = GameManager.instance.playerMaxHungry;
		hungry = GameManager.instance.playerHungry;
		SetBothHealth(GameManager.instance.playerLifePoints, GameManager.instance.playerMaxLifePoints);
		str = GameManager.instance.playerStrPoints;
		def = GameManager.instance.playerDefPoints;
		dex = GameManager.instance.playerDexPoints;
		spd = GameManager.instance.playerSpdPoints;
		luc = GameManager.instance.playerLucPoints;

		base.Start();
		Inventory.Inv.LoadInventory();	
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable()
	{
		//When Player object is disabled, store the current stats in the GameManager so it can be re-loaded in next level.
		GameManager.instance.playerMaxHungry = maxHungry;
		GameManager.instance.playerHungry = hungry;
		GameManager.instance.playerLifePoints = life;
		GameManager.instance.playerMaxLifePoints = maxLife;
		GameManager.instance.playerStrPoints = str;
		GameManager.instance.playerDefPoints = def;
		GameManager.instance.playerDexPoints = dex;
		GameManager.instance.playerSpdPoints = spd;
		GameManager.instance.playerLucPoints = luc;
	}

	private void Update()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int)(Input.GetAxisRaw("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int)(Input.GetAxisRaw("Vertical"));

		//Avoid diagonal movement.
		if (horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if (horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove<Destuctible>(horizontal, vertical);
		}
	}

	private void SetBothHealth(int current, int max)
	{
		maxLife = max;
		life = current;
		HandleHealth();
	}

	private void SetHealth(int value)
	{
		life = value;
		HandleHealth();
	}

	private void SetMaxHealth(int value)
	{
		maxLife = value;
		HandleHealth();
	}

	private void HandleHealth()
	{
		healthText.text = "Health: " + life;

		float currentXValue = MapValues(life, 0, totalMaxLife, minXValue, maxXValue);

		healthTransform.position = new Vector3(currentXValue, cachedY, 0);

		if(life > maxLife * .5f) //More health than 50%
		{
			visualHealth.color = new Color32((byte)MapValues(life, totalMaxLife / 2, totalMaxLife, 255, 0), 255, 0, 255);
		}
		else //Health less than 50%
		{
			visualHealth.color = new Color32(255, (byte)MapValues(life, 0, totalMaxLife / 2, 0, 255), 0, 255);
		}
	}

	private void HandleHungry()
	{
		int h = maxHungry - hungry;
		if (h > maxHungry * .5f) //More hungry than 50%
		{
			hungryIcon.color = new Color32((byte)MapValues(h, maxHungry / 2, maxHungry, 255, 0), 255, 0, 255);
		}
		else //Hungry less than 50%
		{
			hungryIcon.color = new Color32(255, (byte)MapValues(h, 0, maxHungry / 2, 0, 255), 0, 255);
		}
	}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		//Every time player moves, the hungry is increased.
		if (hungry < maxHungry)
		{
			hungry++;
			HandleHungry();
		} else
		{
			SetHealth(--life);
		}

		base.AttemptMove<T>(xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move(xDir, yDir, out hit))
		{
			//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}
		CheckIfGameOver();
		GameManager.instance.playersTurn = false;
	}

	public void Eat(int hungry)
	{
		this.hungry = Mathf.Max(0,this.hungry-hungry);
		HandleHungry();
	}

	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove<T>(T component)
	{
		Destuctible destuctible = component as Destuctible;
		destuctible.LoseLife(this.totalStr, this.totalDex, this.totalLuc);

		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger("playerChop");
	}


	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit")
		{
			Inventory.Inv.SaveInventory();
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}

		else if (other.tag == "Item")
		{
			if (Inventory.Inv.AddItem(other.GetComponent<Item>()))
			{
				SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
				other.gameObject.SetActive(false);
			}
		}
	}

	//Restart reloads the scene when called.
	private void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ObtainLife(int life)
	{
		if (this.life + life <= this.totalMaxLife)
		{
			SetHealth(this.life + life);
		} else
		{
			SetHealth(this.totalMaxLife);
		}
	}

	//LoseLife is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseLife(int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger("playerHit");
		damage.ShowDamage("-"+loss);
		SetHealth(life - loss);

		CheckIfGameOver();
	}

	public void LoseLife(int str, int dex, int luc)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger("playerHit");

		int loss = Random.Range(str - this.totalDef, str - this.totalDef / 2);
		Debug.Log("enemy str " + str + " player def " + this.totalDef);
		Debug.Log("player lose "+(str - this.totalDef) + " - " + (str - this.totalDef / 2));
        loss = Mathf.Max(loss, 1);

		if (Random.Range(0f, 1f) < 1-Mathf.Clamp(this.totalSpd/(dex * 2f), 0f, 0.5f))
		{
			if (Random.Range(0f, 1f) < 1 - Mathf.Clamp(luc / this.totalLuc, 0f, 1f))
			{
				loss += loss;
			}
			damage.ShowDamage("-" + loss);
			SetHealth(life - loss);
		} else
		{
			damage.ShowDamage("dodge");
		}
		

		CheckIfGameOver();
	}


	//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
	private void CheckIfGameOver()
	{
		if (life <= 0)
		{
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}

	public void Use(Item item)
	{
		switch (item.itemType)
		{
			case ItemType.Consumable:
				break;
			default:
				break;
		}
	}

	public void SetStats(int mLife, int str, int def, int dex, int spd, int luc)
	{
		this.totalMaxLife = this.maxLife + mLife;
		this.totalStr = this.str + str;
		this.totalDef = this.def + def;
		this.totalDex = this.dex + dex;
		this.totalSpd = this.spd + spd;
		this.totalLuc = this.luc + luc;

		Inventory.Inv.updateStatsText(string.Format("Life: {0}\nStrength: {1}\nDefense: {2}\nDexterity: {3}\nSpeed: {4}\nLuck: {5}",
			this.totalMaxLife, this.totalStr, this.totalDef, this.totalDex, this.totalSpd, this.totalLuc));

		healthText.text = "Health: " + life;
		HandleHealth();
	}

	public float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
}