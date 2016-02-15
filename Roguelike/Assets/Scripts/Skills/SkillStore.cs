using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TypeSkill
{
	STAT,
	ITEM,
	ACTIVE
}
public class SkillStore : MonoBehaviour, IPointerClickHandler
{
	public TypeSkill typeSkill;
	public int baseCost = 100;
	public float mulCost = 2f;
	public bool intBonus = true;
	public float baseValue = 10;
	public float mulValue = 1.3f;

	public Text level;
	public int namePosition;
	public string info;

	private int cost;
	private int currentLevel = 0;
	private int maxLevel = 10;

	void Start()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			PayMoney();
        }
	}

	public void UpdateSkillLevel()
	{
		if (currentLevel < maxLevel)
		{
			currentLevel++;
			level.text = ""+currentLevel;
		}
	}

	private void PayMoney()
	{
		int money = NextLevelCost();
        if (Money.M.GetMoney() > NextLevelCost()) {
			Money.M.SpendMoney(money);
			UpdateSkillLevel();
			this.transform.GetComponentInParent<SkillPanel>().SaveSkillLevels();
        }
	}

	public int GetLevel()
	{
		return currentLevel;
	}

	public void SetLevel(int level)
	{
		currentLevel = level;
		this.level.text = "" + currentLevel;
	}

	public float GetCurrentBonus()
	{
		return LevelBonus(currentLevel);
	}

	private float LevelBonus(int level)
	{
		if(level == 0 && typeSkill.Equals(TypeSkill.STAT))
		{
			return 0f;
		}

		int l = 0;
		if (typeSkill.Equals(TypeSkill.STAT))
		{
			l = -1;
		}

        float value = baseValue * Mathf.Pow(mulValue, level + l);
		if (intBonus)
		{
			value = Mathf.RoundToInt(value);
		}
		return value;
	}

	private int NextLevelCost()
	{
		return Mathf.RoundToInt(baseCost * Mathf.Pow(mulCost, currentLevel));
    }

	public string GetCurrentSkillInfo()
	{
		string newLine = string.Empty;
		string color = "black";
		string[] names = { };
		switch (typeSkill)
		{
			case TypeSkill.ACTIVE:
				color = "red";
				names = SkillNames.ActiveSkills;
				break;
			case TypeSkill.ITEM:
				color = "cyan";
				names = SkillNames.ItemSkills;
				break;
			case TypeSkill.STAT:
				color = "lime";
				names = SkillNames.StatsSkills;
				break;
		}
		string desc = info.Replace("$", "" + LevelBonus(currentLevel));
		string next = info.Replace("$", "" + LevelBonus(currentLevel+1));

		string name = names[namePosition];

		return string.Format("<color="+color+"><size=16>{0}</size></color>"+
			"<size=14><i><color=white>" + '\n' + "{1}</color></i></size>" +
			"<size=14><i><color=" + color + ">" +'\n'+'\n' + "The Next Level: </color></i></size>"+
			"<size=14><i><color=white>" + '\n' + "{2}</color></i></size>"+
            "<size=12><color=yellow>" + '\n' + '\n' + "Cost: {3}</color></size>", 
			name, desc, next, NextLevelCost());
	}
}
