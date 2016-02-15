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
				Player p = GameObject.Find("Player").GetComponent<Player>();
				p.Eat(hungry);
				p.ObtainLife(life);
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
		Player p = GameObject.Find("Player").GetComponent<Player>();
        int hungryBonus = Mathf.RoundToInt(hungry * p.GetSkillBonus(0));
		int lifeBonus = Mathf.RoundToInt(life * p.GetSkillBonus(1));
		int strBonus = Mathf.RoundToInt(str * p.GetSkillBonus(2));
		int defBonus = Mathf.RoundToInt(def * p.GetSkillBonus(3));
		int dexBonus = Mathf.RoundToInt(dex * p.GetSkillBonus(4));
		int spdBonus = Mathf.RoundToInt(spd * p.GetSkillBonus(5));
		int lucBonus = Mathf.RoundToInt(luc * p.GetSkillBonus(6));

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
			if(life != lifeBonus)
			{
				stats += " (" + lifeBonus + " with skills)";
			}
		}
		if (hungry > 0)
		{
			stats += "\n Takes away hunger ("+hungry+" points)";
			if (hungry != hungryBonus)
			{
				stats += " (" + hungryBonus + " with skills)";
			}
		}
		if (maxLife > 0)
		{
			stats += "\n+" + maxLife.ToString() + " Max Life"; 
		}
		if (str > 0)
		{
			stats += "\n+" + str.ToString() + " Strength";
			if (str != strBonus)
			{
				stats += " (" + strBonus + " with skills)";
			}
		}
		if (def > 0)
		{
			stats += "\n+" + def.ToString() + " Defense";
			if (def != defBonus)
			{
				stats += " (" + defBonus + " with skills)";
			}
		}
		if (dex > 0)
		{
			stats += "\n+" + dex.ToString() + " Dexterity";
			if (dex != dexBonus)
			{
				stats += " (" + dexBonus + " with skills)";
			}
		}
		if (spd > 0)
		{
			stats += "\n+" + spd.ToString() + " Speed";
			if (spd != spdBonus)
			{
				stats += " (" + spdBonus + " with skills)";
			}
		}
		if (luc > 0)
		{
			stats += "\n+" + luc.ToString() + " Luck";
			if (luc != lucBonus)
			{
				stats += " (" + lucBonus + " with skills)";
			}
		}

		return string.Format("<color=" + color + "><size=16>{0}</size></color><size=14><i><color=lime>"+newLine+"{1}</color></i>{2}</size>",itemName,itemInfo,stats);
	}

}
