using UnityEngine;
using System.Collections;

public class Boss2 : Enemy {

	private bool bersek = false;
	public override void MoveEnemy()
	{
		base.MoveEnemy();
	}

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		if(this.life < this.maxLife / 3 && !bersek)
		{
			BerserkSkill();
			return;
		} 
		base.AttemptMove<T>(xDir, yDir);
	}

	private void BerserkSkill()
	{
		GetComponent<SpriteRenderer>().color = Color.red;
		bersek = true;
		this.str *= 2;
		this.dex *= 2;
		this.luc *= 2;
	}
}
