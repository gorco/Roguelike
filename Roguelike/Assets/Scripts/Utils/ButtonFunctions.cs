﻿using UnityEngine;
using System.Collections;

public class ButtonFunctions : MonoBehaviour {

	public void ChangeScene(string name)
	{
		Application.LoadLevel(name);
	}

	public void ChangeScene(int n)
	{
		Application.LoadLevel(n);
	}
}