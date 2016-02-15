using UnityEngine;
using System.Collections;

public interface Destuctible {

	void LoseLife(int damage);

	void LoseLife(int str, int dex, int luc);
}
