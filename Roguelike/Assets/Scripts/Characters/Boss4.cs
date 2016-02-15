using UnityEngine;
using System.Collections;

public class Boss4 : Enemy {

	[Header("Special Attack Appearence")]
	public GameObject attack;

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		
		//If the player isn't contiguous 
		if (hit.transform == null)
		{
			int random = Random.Range(0, 100);
			//Special attack with probability 30%
			if (random < 30)
			{
				SpAttack();
			}
			return;
		}

		//Normal Attack
		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null)
			OnCantMove<T>(hitComponent);

	}

	//Special Attack with infinite range
	protected void SpAttack()
	{
		animator.SetTrigger("enemySp");

		GameObject e = Instantiate(attack, target.transform.position, Quaternion.identity) as GameObject;
		DestroyObject(e, 1f);

		Player p = target.GetComponent<Player>();
		p.LoseLife(p.maxLife / 10);
		
		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
	}
}
