using UnityEngine;
using System.Collections;

public class Boss1 : Enemy {

	[Header("Special Attack fields")]
	public int playerDamage;                           
	public int spTurns = 3;                          //The number of turns between special attacks.

	private int count = 0;

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		//Normal attack;
		if(count < spTurns)
		{
			base.AttemptMove<T>(xDir, yDir);
			count++;
			return;
		}

		//Special Attack
		count = 0;
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);

		if (hit.transform == null)
			return;

		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null)
			OnCantMoveSp(hitComponent);
		
	}

	protected void OnCantMoveSp<T>(T component)
	{
		Destuctible hitPlayer = component as Destuctible;
		hitPlayer.LoseLife(playerDamage);

		animator.SetTrigger("enemySp");

		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
	}

}
