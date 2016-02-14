using UnityEngine;
using System.Collections;

public class Boss4 : Enemy {

	public GameObject attack;

	public override void MoveEnemy()
	{
		Debug.Log("MoveEnemyBoss");
		base.MoveEnemy();
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		
		if (hit.transform == null)
		{
			int random = Random.Range(0, 100);
			if (random < 30)
			{
				SpAttack();
			}
			return;
		}

		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null)
			OnCantMove<T>(hitComponent);

	}

	protected void SpAttack()
	{
		animator.SetTrigger("enemySp");
		GameObject e = Instantiate(attack, target.transform.position, Quaternion.identity) as GameObject;
		DestroyObject(e, 1f);
		Player p = target.GetComponent<Player>();
		p.LoseLife(p.maxLife / 10);

		//Set the attack trigger of animator to trigger Enemy attack animation.
		
		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
	}
}
