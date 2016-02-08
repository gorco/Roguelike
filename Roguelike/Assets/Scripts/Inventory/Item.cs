using UnityEngine;

public enum ItemType
{
	Head,
	Armor,
	Gloves,
	Boots,
	Weapon,
	Shield,
	Consumable,
	Potions
}

public enum Quality
{
	Common, 
	Uncommon, 
	Rare,
	Epic
}

public class Item : MonoBehaviour {

	public Sprite itemSprite;
	public ItemType itemType;
	public Quality quality;

	public string itemName;
	public string itemInfo;

	public int power;
	public int value;

	public int hungry;
	public int life;
	public int maxLife;
	public int str;
	public int def;
	public int dex;
	public int spd;
	public int luc;

	public void Use ()
	{
		switch (itemType)
		{
			case ItemType.Consumable:
				GameObject.Find("Player").GetComponent<Player>().Eat(hungry);
				break;
			case ItemType.Potions:
				GameObject.Find("Player").GetComponent<Player>().ObtainLife(life);
				break;
			default:
				break;
		}
	}

	public bool CanBeEquiped()
	{
		if (itemType != ItemType.Consumable && itemType != ItemType.Potions)
		{
			return true;
		}

		return false;
	}

	public bool CanBeConsumed()
	{
		if (itemType == ItemType.Consumable || itemType == ItemType.Potions)
		{
			return true;
		}

		return false;
	}

	public bool CanBeUsed()
	{
		return false;
	}

	public string GetTooltip()
	{
		string stats = string.Empty;
		string color = string.Empty;
		string newLine = string.Empty;

		if(itemInfo != string.Empty)
		{
			newLine = "\n";
		}

		switch (quality)
		{
			case Quality.Common:
				color = "white";
				break;
			case Quality.Uncommon:
				color = "green";
				break;
			case Quality.Rare:
				color = "navy";
				break;
			case Quality.Epic:
				color = "orange";
				break;
		}

		if (life > 0)
		{
			stats += "\n Heal " + life.ToString() + " points of life";
		}
		if (maxLife > 0)
		{
			stats += "\n+" + maxLife.ToString() + " Max Life"; 
		}
		if (str > 0)
		{
			stats += "\n+" + str.ToString() + " Strength";
		}
		if (def > 0)
		{
			stats += "\n+" + def.ToString() + " Defense";
		}
		if (dex > 0)
		{
			stats += "\n+" + dex.ToString() + " Dexterity";
		}
		if (spd > 0)
		{
			stats += "\n+" + spd.ToString() + " Speed";
		}
		if (luc > 0)
		{
			stats += "\n+" + luc.ToString() + " Luck";
		}

		return string.Format("<color=" + color + "><size=16>{0}</size></color><size=14><i><color=lime>"+newLine+"{1}</color></i>{2}</size>",itemName,itemInfo,stats);
	}

}
