﻿using UnityEngine;

public class ButtonFunctions : MonoBehaviour {

	public void ChangeScene(string name)
	{
		Application.LoadLevel(name);
	}

	public void ChangeScene(int n)
	{
		Application.LoadLevel(n);
	}

	public void DestroyGameManager()
	{
		DestroyObject(GameManager.instance.gameObject, 1f);
	}
}
