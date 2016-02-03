using UnityEngine;
using System.Collections;

public interface Destuctible {

	// Use this for initialization
	void LoseLife(int damage);

	void LoseLife(int str, int dex, int luc);
}
