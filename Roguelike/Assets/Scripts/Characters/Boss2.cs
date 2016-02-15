using UnityEngine;
using System.Collections;

public class Boss2 : Enemy {

	private bool berserk = false;

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		//Special move
		if(this.life < this.maxLife / 3 && !berserk)
		{
			BerserkSkill();
			return;
		} 

		//Normal attack
		base.AttemptMove<T>(xDir, yDir);
	}

	//Active berserk mode
	private void BerserkSkill()
	{
		GetComponent<SpriteRenderer>().color = Color.red;
		berserk = true;
		this.str *= 2;
		this.dex *= 2;
		this.luc *= 2;
	}
}
