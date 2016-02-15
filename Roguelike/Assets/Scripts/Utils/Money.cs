using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Money : MonoBehaviour {

	[Header("Money Text")]
	public Text money;
	private int coins = 0;

	public static Money M;

	void Start () {
		M = this;
		this.money.text = PlayerPrefs.GetString("Money");
		if (string.IsNullOrEmpty(this.money.text))
		{
			PlayerPrefs.SetString("Money", "" + 0);
			PlayerPrefs.Save();
			coins = 0;
			this.money.text = "0";
		}
		else
		{
			coins = int.Parse(this.money.text);
		}
		UpdateMoney();
		
	}
	
	public void MoreMoney(int money)
	{
		coins += money;
		UpdateMoney();
    }

	public void SpendMoney(int money)
	{
		coins -= money;
		UpdateMoney();
    }

	private void UpdateMoney()
	{
		this.money.text = "" + coins;
		PlayerPrefs.SetString("Money", "" + coins);
		PlayerPrefs.Save();
	}

	public int GetMoney()
	{
		return coins;
	}
}
