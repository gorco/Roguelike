using UnityEngine;
using System.Collections;

public class Boss5 : Enemy {

	public int maxHealTimes = 5;

	public override void MoveEnemy()
	{
		base.MoveEnemy();
	}

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		Debug.Log(this.life + " _ " + this.maxLife / 4);
		if(this.life < this.maxLife / 4 && maxHealTimes != 0)
		{
			HealSkill();
			return;
		} 
		base.AttemptMove<T>(xDir, yDir);
	}

	private void HealSkill()
	{
		maxHealTimes--;
		animator.SetTrigger("enemySp");
		this.life += this.maxLife / 2;
		UpdateHealthBar();
	}
}
