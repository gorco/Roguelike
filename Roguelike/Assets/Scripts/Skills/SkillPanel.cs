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

	void Start()
	{
		tooltip = tooltipObject;
		sizeText = sizeTextObject;
		visualText = visualTextObject;
	}

	public void ShowTooltip(GameObject slot)
	{
		SkillStore tmpSlot = slot.GetComponent<SkillStore>();

		if (this.enabled == true)
		{
			visualText.text = tmpSlot.GetCurrentSkillInfo();
			sizeText.text = visualText.text;

			tooltip.SetActive(true);

			float xPos = slot.transform.position.x + 1;
			float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - topPad;

			tooltip.transform.position = new Vector2(xPos, yPos);
		}

	}

	public void HideTooltip(GameObject slot)
	{
		tooltip.SetActive(false);
	}

	public void SavePanel()
	{
		/*
		string content = string.Empty;

		for (int i = 0; i < slotsList.Count; i++)
		{
			if (!slotsList[i].IsEmpty())
			{
				Item item = slotsList[i].GetCurrentItem();
				//[0] = slotPosition
				//[1] = itemName
				//[2] = itemType
				//[3] = itemPower
				content += i + "/" + item.itemName + "/" + item.itemType.ToString() + "/" + item.power + ";";
			}
			else
			{
				content += i + "/" + "empty;";
			}
		}

		PlayerPrefs.SetString("InventoryContent", content);
		PlayerPrefs.Save();
		*/
	}

	public void LoadInventory()
	{
		/*
		string equipment = PlayerPrefs.GetString("EquipmentContent");
		if (equipment != string.Empty)
		{
			string[] splitEquipment = equipment.Split(';');
			foreach (string savedItem in splitEquipment)
			{
				if (!string.IsNullOrEmpty(savedItem))
				{
					string[] splitValues = savedItem.Split('/');
					if (!splitValues[1].Equals("empty"))
					{
						Item item = GetItemInstance(splitValues[1], splitValues[2], int.Parse(splitValues[3]));
						if (item != null)
						{
							equipmentSlots[int.Parse(splitValues[0])].AddItem(item);
							item.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		*/
	}
}
