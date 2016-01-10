using UnityEngine;
using System.Collections;
using System;

public class Wall : MonoBehaviour, Destuctible
{         
	public Sprite dmgSprite;                    //Alternate sprite to display after Wall has been attacked by player.
	public int hp = 3;                          //hit points for the wall.

	public AudioClip chopSound1;
	public AudioClip chopSound2;

	private SpriteRenderer spriteRenderer;      //Store a component reference to the attached SpriteRenderer.

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}


	//DamageWall is called when the player attacks a wall.
	public void LoseLife(int loss)
	{
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		spriteRenderer.sprite = dmgSprite;
		hp -= loss;

		if (hp <= 0)
			gameObject.SetActive(false);
	}

	public void LoseLife(int str, int dex)
	{
	}
}