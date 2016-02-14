using UnityEngine;
using System.Collections;

public class Boss1 : Enemy {

	private bool bersek = false;
	public int spTurns = 3;
	private int count = 0;

	public override void MoveEnemy()
	{
		base.MoveEnemy();
	}

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		if(count < spTurns)
		{
			base.AttemptMove<T>(xDir, yDir);
			count++;
			return;
		}

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
		//Set the attack trigger of animator to trigger Enemy attack animation.
		animator.SetTrigger("enemySp");

		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
	}

}
