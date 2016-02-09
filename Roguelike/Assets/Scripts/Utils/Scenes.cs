using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour {

	public void ChangeScene(string name)
	{
		SceneManager.LoadScene(name);
	}
}
