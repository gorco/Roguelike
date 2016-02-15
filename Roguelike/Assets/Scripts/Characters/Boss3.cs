using UnityEngine;
using System.Collections.Generic;

public class Boss3 : Enemy {

	[Header("Special Attack fields")]
	public int spTurns = 3;
	private int count = 0;

	private GameObject enemiesStore;
	private List<Vector3> positions = new List<Vector3>();

	[Header("Summoned enemies")]
	public GameObject enemy;

	protected override void AttemptMove<T>(int xDir, int yDir) 
	{
		//Initialize enemiesStore
		if(enemiesStore == null)
		{
			enemiesStore = GameObject.Find("EnemiesStore");
			Debug.Log("Childs" + enemiesStore.transform.childCount);
			for (int i = 0; i < enemiesStore.transform.childCount; i++)
			{
				positions.Add(enemiesStore.transform.GetChild(i).transform.position);
			}
		}

		//Summon enemies (Special move)
		if (enemiesStore.transform.childCount == 0)
		{
			if (count == spTurns)
			{
				animator.SetTrigger("enemySp");
				int i = 0;
				foreach (Vector3 pos in positions)
				{
					GameObject e = Instantiate(enemy, positions[i], Quaternion.identity) as GameObject;
					e.transform.SetParent(enemiesStore.transform);
					i++;
				}
				count = 0;
				return;
			}
			count++;
		}

		//Normal attack
		base.AttemptMove<T>(xDir, yDir);
	}

	public override void LoseLife(int loss)
	{
		//The boss is invulnerable while there are others enemies
		if (enemiesStore.transform.childCount > 0)
		{
			base.LoseLife(0);
			return;
		}

		base.LoseLife(loss);
	}

	public override void LoseLife(int str, int dex, int luc)
	{
		//The boss is invulnerable while there are others enemies
		if (enemiesStore.transform.childCount > 0)
		{
			base.LoseLife(0);
			return;
		}
		base.LoseLife(str, dex, luc);
	}
}
