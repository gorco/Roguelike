using UnityEngine;
using System.Collections;

public class Enemy : MovingObject, Destuctible
{
	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.

	protected Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	protected Transform target;                           //Transform to attempt to move toward each turn.
	private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.

	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	public RectTransform healthBar;

	//Start overrides the virtual Start function of the base class.
	protected override void Start()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
		//This allows the GameManager to issue movement commands.
		GameManager.instance.AddEnemyToList(this);

		animator = GetComponent<Animator>();

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag("Player").transform;

		base.Start();
	}

	//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
	//See comments in MovingObject for more on how base AttemptMove function works.
	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		if (skipMove)
		{
			skipMove = false;
			return;

		}
		base.AttemptMove<T>(xDir, yDir);
		//skipMove = true;
	}

	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public virtual void MoveEnemy()
	{
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;

		//If the difference in positions is approximately zero (Epsilon) do the following:
		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

			//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
			yDir = target.position.y > transform.position.y ? 1 : -1;

		//If the difference in positions is not approximately zero (Epsilon) do the following:
		else
			//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
			xDir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player>(xDir, yDir);
	}

	//It takes a parameter loss which specifies how many points to lose.
	public virtual void LoseLife(int loss)
	{
		damage.ShowDamage(""+loss);
		life -= loss;
		UpdateHealthBar();
		CheckIfDie();
	}

	public virtual void LoseLife(int str, int dex, int luc)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		int loss = Random.Range(str - this.def, str - this.def / 2);
		loss = Mathf.Max(loss, 1);

		if (Random.Range(0f, 1f) < 1 - Mathf.Clamp(this.spd / (dex * 2f), 0f, 0.5f))
		{
			if (Random.Range(0f, 1f) < 1 - Mathf.Clamp(luc / this.luc, 0f, 1f))
			{
				loss += loss;
			}

			damage.ShowDamage("-" + loss);
			this.life -= loss;
			UpdateHealthBar();
		}
		else
		{
			damage.ShowDamage("dodge");
		}

		CheckIfDie();
	}

	protected void UpdateHealthBar()
	{
		float x = Mathf.Clamp(this.life / (this.maxLife * 1f), 0, 1);
		healthBar.transform.localScale = new Vector3(x, 1, 1);
	}

	public void CheckIfDie()
	{
		if (life <= 0)
		{
			DestroyObject(this.gameObject, 0.25f);
			GameManager.instance.RemoveEnemyToList(this);
		}
	}

	//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
	//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
	protected override void OnCantMove<T>(T component)
	{
		Destuctible hitPlayer = component as Destuctible;
		hitPlayer.LoseLife(str, dex, luc);
		//Set the attack trigger of animator to trigger Enemy attack animation.
		animator.SetTrigger("enemyAttack");

		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
	}
}