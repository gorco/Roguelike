using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillPanel : MonoBehaviour
{
	public GameObject tooltipObject;
	public Text sizeTextObject;
	public Text visualTextObject;

	private static GameObject tooltip;
	private static Text sizeText;
	private static Text visualText;

	private int topPad = 2;

	private SkillStore auxSlot;

	private List<SkillStore> slotsList = new List<SkillStore>();

	void Start()
	{
		tooltip = tooltipObject;
		sizeText = sizeTextObject;
		visualText = visualTextObject;

		for(int i = 0; i < this.transform.childCount; i++)
		{
			slotsList.Add(this.transform.GetChild(i).GetComponent<SkillStore>());
		}

		LoadSkillLevels();
	}

	public void ShowTooltip(GameObject slot)
	{
		SkillStore tmpSlot = slot.GetComponent<SkillStore>();

		if (this.enabled == true)
		{
			visualText.text = tmpSlot.GetCurrentSkillInfo();
			sizeText.text = visualText.text;

			tooltip.SetActive(true);
			auxSlot = tmpSlot;

			
			float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - topPad;
			if (tmpSlot.typeSkill == TypeSkill.ITEM)
			{
				float xPos = slot.transform.position.x + slot.GetComponent<RectTransform>().sizeDelta.x;
				tooltip.transform.position = new Vector2(xPos, yPos);
				Invoke("CorrectPosition", 0.05f);
			} else
			{
				float xPos = slot.transform.position.x + 1;
				tooltip.transform.position = new Vector2(xPos, yPos);
			}
		}
	}

	private void CorrectPosition()
	{
		tooltip.transform.position = new Vector2(tooltip.transform.position.x - sizeText.GetComponent<RectTransform>().sizeDelta.x, tooltip.transform.position.y);
	}

	public void HideTooltip(GameObject slot)
	{
		auxSlot = null;
		tooltip.SetActive(false);
	}

	public void SaveSkillLevels()
	{
		string content = string.Empty;

		for (int i = 0; i < slotsList.Count; i++)
		{
			content += slotsList[i].namePosition + "/" + slotsList[i].GetLevel()+"/"+slotsList[i].GetCurrentBonus().ToString()+";";
		}

		PlayerPrefs.SetString(this.name, content);
		PlayerPrefs.Save();
	}

	public void LoadSkillLevels()
	{
		string content = PlayerPrefs.GetString(this.name);
		if (content != string.Empty)
		{
			string[] splitContent = content.Split(';');
			foreach (string line in splitContent)
			{
				if (!string.IsNullOrEmpty(line))
				{
					string[] splitValues = line.Split('/');
					int l = int.Parse(splitValues[1]);
					if (l!=0)
					{
						int pos = int.Parse(splitValues[0]);
						foreach(SkillStore s in slotsList)
						{
                            if (s.namePosition == pos)
							{
								s.SetLevel(l);
							}
						}
					}
				}
			}
		}
	}
}
