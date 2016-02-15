using UnityEngine;
using System.Collections;

public class Boss5 : Enemy {

	[Header("Max Special Attacks")]
	public int maxHealTimes = 5;

	//The enemy copy the player stats
	protected override void Start()
	{
		base.Start();
		Player p = target.GetComponent<Player>();
		p.CopyValues(out maxLife, out str, out def, out dex, out spd, out luc);
		this.life = this.maxLife;
	}

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		//If boss health is low and he can use the special attack
		if(this.life < this.maxLife / 4 && maxHealTimes != 0)
		{
			HealSkill();
			return;
		} 
		base.AttemptMove<T>(xDir, yDir);
	}

	//The boss heal hisself
	private void HealSkill()
	{
		maxHealTimes--;
		animator.SetTrigger("enemySp");
		this.life += this.maxLife / 2;
		UpdateHealthBar();
	}
}
