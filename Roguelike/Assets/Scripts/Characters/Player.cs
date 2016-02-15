using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject, Destuctible
{
	[Header("Level Charge Delay")]
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.

	[Header("Sounds")]
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;                  //Used to store a reference to the Player's animator component.

	private int hungry;
	private int maxHungry;
	private int totalMaxLife;
	private int totalStr;
	private int totalDef;
	private int totalDex;
	private int totalSpd;
	private int totalLuc;

	[Header("Health Bar HUD")]
	public RectTransform healthTransform;
	public Text healthText;
	public Image visualHealth;
	private float cachedY;
	private float minXValue;
	private float maxXValue;

	[Header("Hungry HUD")]
	public Image hungryIcon;

	[Header("Key HUD")]
	public Text keyCountText;
	private int keyCount;
	public CanvasGroup KeyNeeded;

	private List<float> itemsBonus = new List<float>();

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
		itemsBonus = GameManager.instance.itemsBonus;

		base.Start();
		Inventory.Inv.LoadInventory();
		HandleHungry();
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

	//For Boss5 features
	public void CopyValues(out int maxLife, out int str, out int def, out int dex, out int spd, out int luc)
	{
		maxLife = this.totalMaxLife;
		str = this.totalStr;
		def = this.totalDef;
		dex = this.totalDex;
		spd = this.totalSpd;
		luc = this.totalLuc;
	}

	public float GetSkillBonus(int pos)
	{
		Debug.Log("Bonus" + pos + " = " + itemsBonus[pos]);
		return itemsBonus[pos];
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
		int hungryBonus = Mathf.RoundToInt(hungry * GetSkillBonus(0));
		this.hungry = Mathf.Max(0,this.hungry - hungryBonus);
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
			if(keyCount > 0)
			{
				Inventory.Inv.SaveInventory();
				keyCount--;
				keyCountText.text = "x" + keyCount;
				Invoke("Restart", restartLevelDelay);
				enabled = false;
			}
			else
			{
				KeyNeeded.alpha = 1f;
				Invoke("HideKeyMessage", 2f);
			}
			return;
		}

		if (other.tag == "Item")
		{
			if (Inventory.Inv.AddItem(other.GetComponent<Item>()))
			{
				SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
				other.gameObject.SetActive(false);
			}
		}
	}

	public void ObtainKey()
	{
		keyCount++;
		keyCountText.text = "x" + keyCount;
	}

    private void HideKeyMessage()
	{
		KeyNeeded.alpha = 0f;
	}

	//Restart reloads the scene when called.
	private void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ObtainLife(int life)
	{
		int lifeBonus = Mathf.RoundToInt(life * GetSkillBonus(1));
		if (this.life + lifeBonus <= this.totalMaxLife)
		{
			SetHealth(this.life + lifeBonus);
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

		//Calculate Damage
		int loss = Random.Range(str - this.totalDef, str - this.totalDef / 2);
        loss = Mathf.Max(loss, 1);

		//Probability of Dodge
		if (Random.Range(0f, 1f) < 1-Mathf.Clamp(this.totalSpd/(dex * 2f), 0f, 0.5f))
		{
			//Probability of Critical attack
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

	//Update stats with the equipment
	public void SetStats(int mLife, int str, int def, int dex, int spd, int luc)
	{
		int strBonus = Mathf.RoundToInt(str * GetSkillBonus(2));
		int defBonus = Mathf.RoundToInt(def * GetSkillBonus(3));
		int dexBonus = Mathf.RoundToInt(dex * GetSkillBonus(4));
		int spdBonus = Mathf.RoundToInt(spd * GetSkillBonus(5));
		int lucBonus = Mathf.RoundToInt(luc * GetSkillBonus(6));

		this.totalMaxLife = this.maxLife + mLife;
		this.totalStr = this.str + strBonus;
		this.totalDef = this.def + defBonus;
		this.totalDex = this.dex + dexBonus;
		this.totalSpd = this.spd + spdBonus;
		this.totalLuc = this.luc + lucBonus;

		Inventory.Inv.updateStatsText(string.Format("Life: {0}\nStrength: {1}\nDefense: {2}\nDexterity: {3}\nSpeed: {4}\nLuck: {5}",
			this.totalMaxLife, this.totalStr, this.totalDef, this.totalDex, this.totalSpd, this.totalLuc));

		healthText.text = "Health: " + life;
		HandleHealth();
	}

	//For HealthBar and HungryIcon
	public float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
}