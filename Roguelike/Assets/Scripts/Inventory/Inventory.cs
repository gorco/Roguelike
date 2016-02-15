using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

	public Slot slotPrefab;
	public int outPading = 5;
	public int slots = 25;
	public int rows = 5;
	public int leftPad = 2;
	public int topPad = 2;

	public GameObject iconPrefab;

	private float inventoryW, inventoryH;
	private List<Slot> slotsList;

	private static Slot from, to;

	private int emptySlots = 0;

	private static GameObject hoverObject;
	public Canvas canvas;

	public static Inventory Inv;

	public EventSystem eventSystem;

	public UnityEngine.UI.Image equipmentPrefab;

	private UnityEngine.UI.Image equipment;

	private UnityEngine.UI.Image inventoryBar;
	public UnityEngine.UI.Image close;
	public UnityEngine.UI.Image statsPanel;

	public GameObject tooltipObject;
	public Text sizeTextObject;
	public Text visualTextObject;

	private Text statsText;
	private static GameObject tooltip;
	private static Text sizeText;
	private static Text visualText;

	private CanvasGroup canvasGroup;

	private Vector3 oneSize = new Vector3(1, 1, 1);

	private List<Slot> equipmentSlots;

	public GameObject[] potionsTiles;

	public GameObject[] consumibleTiles1;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles1;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles1;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles2;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles2;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles2;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles3;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles3;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles3;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles4;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles4;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles4;                              //Array of weapons prefabs.

	public GameObject[] consumibleTiles5;                           //Array of consumible prefabs.
	public GameObject[] equipmentTiles5;                            //Array of equipment prefabs.
	public GameObject[] weaponsTiles5;                              //Array of weapons prefabs.

	void Awake ()
	{
		Inv = this;
		slotsList = new List<Slot>();
		equipmentSlots = new List<Slot>();

		tooltip = tooltipObject;
		sizeText = sizeTextObject;
		visualText = visualTextObject;

		canvasGroup = GetComponent<CanvasGroup>();
		CreateInventoryLayout();
    }

	void Start()
	{
		OpenInventory(false);
	}

	private void CreateInventoryLayout()
	{
		int columns = slots / rows;
		Vector3 slotSize = slotPrefab.GetComponent<RectTransform>().rect.size;

		inventoryW = (slotSize.x + leftPad) * columns + outPading * 2 - leftPad;
		inventoryH = (slotSize.y + topPad) * rows + outPading * 2 - topPad;

		RectTransform inventoryRect = this.GetComponent<RectTransform>();
		inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryW);
		inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryH);

		float orgX = -inventoryW / 2;
		float orgY = inventoryH / 2;
		for (int row = 0; row < rows; row++)
		{
			for (int column = 0; column < columns; column++)
			{
				Slot slot = Instantiate(slotPrefab);
				slot.transform.SetParent(this.gameObject.transform);
				slot.transform.localScale = oneSize;
				slot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading + (leftPad + slotSize.x) * column, orgY - outPading - (topPad + slotSize.y) * row, 0);
				slotsList.Add(slot);
				emptySlots++;
			}
		}

		//Equipment Panel
		equipment = Instantiate(equipmentPrefab);
		equipment.transform.SetParent(this.transform);
		equipment.transform.localScale = oneSize;

		float equipmentW = slotSize.x * 2 + outPading * 5;

		RectTransform equipmentRect = equipment.GetComponent<RectTransform>();
		equipmentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, equipmentW);
		equipmentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryH);

		equipmentRect.localPosition = new Vector3(-inventoryW + equipmentW/2, 0,0);

		//InventoryBar
		inventoryBar = Instantiate(equipmentPrefab);
		inventoryBar.transform.SetParent(this.transform);
		inventoryBar.transform.localScale = oneSize;

		float barW = equipmentW + inventoryW + leftPad;

		RectTransform barRect = inventoryBar.GetComponent<RectTransform>();
		barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barW);
		barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 36);

		barRect.localPosition = new Vector3(-(equipmentW+outPading)/2, (inventoryH + 36)/2, 0);

		close.transform.SetParent(inventoryBar.transform);
		close.transform.localScale = oneSize;
		RectTransform closeRect = close.GetComponent<RectTransform>();
		closeRect.localPosition = new Vector3(barW/2-closeRect.sizeDelta.x-leftPad, 0, 0);

		//StatsPanel 
		RectTransform statsRect = statsPanel.GetComponent<RectTransform>();
		statsRect.transform.SetParent(this.transform);
		statsRect.localPosition = new Vector3(inventoryW/2, 0, 0);

		statsText = statsPanel.GetComponentInChildren<Text>();

		//Equipment Slots
		orgX = -equipmentW / 2;

		//Head slot
		Slot helmetSlot = Instantiate(slotPrefab);
		helmetSlot.transform.SetParent(equipment.transform);
		helmetSlot.transform.localScale = oneSize;
		helmetSlot.AddSpecialization(ItemType.Head);
		helmetSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading*2.5f + slotSize.x/2, orgY - outPading*2, 0);
		equipmentSlots.Add(helmetSlot);

		//Weapon slot
		Slot weaponSlot = Instantiate(slotPrefab);
		weaponSlot.transform.SetParent(equipment.transform);
		weaponSlot.transform.localScale = oneSize;
		weaponSlot.AddSpecialization(ItemType.Weapon);
		weaponSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2, orgY - outPading * 2 - slotSize.y - outPading, 0);
		equipmentSlots.Add(weaponSlot);

		//Shield slot
		Slot shieldSlot = Instantiate(slotPrefab);
		shieldSlot.transform.SetParent(equipment.transform);
		shieldSlot.transform.localScale = oneSize;
		shieldSlot.AddSpecialization(ItemType.Shield);
		shieldSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 3 + slotSize.x, orgY - outPading * 2 - slotSize.y - outPading, 0);
		equipmentSlots.Add(shieldSlot);

		//Armor slot
		Slot armorSlot = Instantiate(slotPrefab);
		armorSlot.transform.SetParent(equipment.transform);
		armorSlot.transform.localScale = oneSize;
		armorSlot.AddSpecialization(ItemType.Armor);
		armorSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2, orgY - outPading * 2 - (slotSize.y + outPading)*2, 0);
		equipmentSlots.Add(armorSlot);

		//Hands slot
		Slot hadsSlot = Instantiate(slotPrefab);
		hadsSlot.transform.SetParent(equipment.transform);
		hadsSlot.transform.localScale = oneSize;
		hadsSlot.AddSpecialization(ItemType.Gloves);
		hadsSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 3 + slotSize.x, orgY - outPading * 2 - (slotSize.y + outPading) * 2, 0);
		equipmentSlots.Add(hadsSlot);

		//Shoes slot
		Slot shoesSlot = Instantiate(slotPrefab);
		shoesSlot.transform.SetParent(equipment.transform);
		shoesSlot.transform.localScale = oneSize;
		shoesSlot.AddSpecialization(ItemType.Boots);
		shoesSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2.5f + slotSize.x / 2, orgY - outPading * 2 - (slotSize.y + outPading) * 3, 0);
		equipmentSlots.Add(shoesSlot);

		//Set final inventory position (centered)
		Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
        this.transform.position = new Vector3(canvasRect.width/2, canvasRect.height / 2, 0);
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			//Remove/drop items if clicking out of inventory and item is selected
			if (!eventSystem.IsPointerOverGameObject(-1) && from != null)
			{
				if (from.IsSpecialized())
				{
					from.RemoveItem();
					CalcStats();
				} else
				{
					from.RemoveItem();
					IncreaseEmptyCount();
				}
				Destroy(hoverObject);
				ResetInventoryState();
			}
		}

		// Move hover object around the canvas
		if (hoverObject != null)
		{
			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.GetComponent<Camera>(), out position);
			position.x += 1;
			position.y += 1;
			hoverObject.transform.position = canvas.transform.TransformPoint(position);
		}
	}

	public void ShowTooltip(GameObject slot)
	{
		Slot tmpSlot = slot.GetComponent<Slot>();
		
		if(this.enabled == true && !tmpSlot.IsEmpty() && hoverObject == null)
		{
			visualText.text = tmpSlot.GetCurrentItem().GetTooltip();
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

	public bool AddItem(Item item)
	{
		if(emptySlots > 0)
		{
			foreach(Slot slot in slotsList)
			{
				if (slot.IsEmpty())
				{
					slot.AddItem(item);
					DecreaseEmptyCount();
					return true;
				}
			}
		}

		return false;
	}

	public void DecreaseEmptyCount()
	{
		emptySlots--;
	}

	public void IncreaseEmptyCount()
	{
		emptySlots++;
	}

	private Slot GetSpecilizedSlot(ItemType type)
	{
		foreach (Slot slot in equipmentSlots)
		{
			if(slot.GetSpecialization() == type)
			{
				return slot;
			}
		}
		return null;
	}

	public void EquipItem(Slot slot)
	{
		Slot spSlot = GetSpecilizedSlot(slot.GetCurrentItem().itemType);
        MoveItem(slot.gameObject);
		MoveItem(spSlot.gameObject);
	}

	public void MoveItem(GameObject clicked)
	{
		if (from == null)
		{
			if (!clicked.GetComponent<Slot>().IsEmpty())
			{
				from = clicked.GetComponent<Slot>();
				from.GetComponent<Image>().color = Color.gray;

				hoverObject = Instantiate(iconPrefab);
				hoverObject.GetComponent<Image>().sprite = from.GetCurrentItem().itemSprite;
				from.HideItem(true); 

				RectTransform hoverTransform = hoverObject.GetComponent<RectTransform>();
				RectTransform clickedTransform = clicked.GetComponent<RectTransform>();

				hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
				hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);

				hoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
				hoverObject.transform.localScale = from.gameObject.transform.localScale;
			}
		}

		else if (to == null)
		{
			to = clicked.GetComponent<Slot>();
			Destroy(hoverObject);
		}

		if(to!=null && from != null)
		{
			if(to.IsSpecialized() && to.IsEmpty())
			{
				IncreaseEmptyCount();
			} else if(from.IsSpecialized() && to.IsEmpty())
			{
				DecreaseEmptyCount();
			}

			Item tmp = to.GetCurrentItem();
			if (to.ChangeItem(from.GetCurrentItem()))
			{
				from.ChangeItem(tmp);
			}
			if (!from.IsEmpty())
			{
				from.HideItem(false);
			}
			if (to.IsSpecialized())
			{
				CalcStats();
			}
			ResetInventoryState();
		}
	}

	private void ResetInventoryState()
	{
		from.GetComponent<Image>().color = Color.white;
		to = null;
		from = null;
		hoverObject = null;
	}

	public void OpenInventory(bool open)
	{
		if (open && canvasGroup.alpha < 0.5f)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		} else if(!open && canvasGroup.alpha >= 0.5f)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
	}

	public void updateStatsText(string text)
	{
		statsText.text = text;
	}

	public void CalcStats()
	{
		Player p = FindObjectOfType<Player>();
		int maxLife = 0;
		int str = 0;
		int def = 0;
		int dex = 0;
		int spd = 0;
		int luc = 0;

		foreach (Slot slot in equipmentSlots)
		{
			if (!slot.IsEmpty())
			{
				Item item = slot.GetCurrentItem();
				maxLife += item.maxLife;
				str += item.str;
				def += item.def;
				dex += item.dex;
				spd += item.spd;
				luc += item.luc;
			}
		}

		p.SetStats(maxLife, str, def, dex, spd, luc);
	}

	public void SaveInventory()
	{
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

		string equipment = string.Empty;
		for (int i = 0; i < equipmentSlots.Count; i++)
		{
			if (!equipmentSlots[i].IsEmpty())
			{
				Item item = equipmentSlots[i].GetCurrentItem();
				//[0] = slotPosition
				//[1] = itemName
				//[2] = itemType
				//[3] = itemPower
				equipment += i + "/" + item.itemName + "/" + item.itemType.ToString() + "/" + item.power + ";";
			}
			else
			{
				equipment += i + "/" + "empty;";
			}
		}

		PlayerPrefs.SetString("InventoryContent", content);
		PlayerPrefs.SetString("EquipmentContent", equipment);
		PlayerPrefs.Save();
	}

	public void LoadInventory()
	{
		
		string equipment = PlayerPrefs.GetString("EquipmentContent");
		if(equipment != string.Empty)
		{
			string[] splitEquipment = equipment.Split(';');
			foreach(string savedItem in splitEquipment)
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

		string content = PlayerPrefs.GetString("InventoryContent");
		if (content != string.Empty)
		{
			string[] splitContent = content.Split(';');
			foreach (string savedItem in splitContent)
			{
				if (!string.IsNullOrEmpty(savedItem))
				{
					string[] splitValues = savedItem.Split('/');
					if (splitValues[1]!="empty")
					{
						Item item = GetItemInstance(splitValues[1], splitValues[2], int.Parse(splitValues[3]));
						if (item != null)
						{
							slotsList[int.Parse(splitValues[0])].AddItem(item);
							item.gameObject.SetActive(false);
						}
					} 
				}
			}
		}
		CalcStats();
	}

	private Item GetItemInstance(string name, string type, int power)
	{
		if (type.Equals(ItemType.Potions.ToString()))
		{
			foreach (GameObject item in potionsTiles)
			{
				if (item.GetComponent<Item>().itemName.Equals(name))
				{
					return Instantiate<GameObject>(item).GetComponent<Item>();
				}
			}
			return null;
		}

		switch (power)
		{
			case 1:
				if (type.Equals(ItemType.Consumable.ToString()))
				{
					foreach (GameObject item in consumibleTiles1)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else if (type.Equals(ItemType.Weapon.ToString()) || type.Equals(ItemType.Shield.ToString()))
				{
					foreach (GameObject item in weaponsTiles1)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else //Equipment
				{
					foreach (GameObject item in equipmentTiles1)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				break;
			case 2:
				if (type.Equals(ItemType.Consumable.ToString()))
				{
					foreach (GameObject item in consumibleTiles2)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else if (type.Equals(ItemType.Weapon.ToString()) || type.Equals(ItemType.Shield.ToString()))
				{
					foreach (GameObject item in weaponsTiles2)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else //Equipment
				{
					foreach (GameObject item in equipmentTiles2)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				break;
			case 3:
				if (type.Equals(ItemType.Consumable.ToString()))
				{
					foreach (GameObject item in consumibleTiles3)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else if (type.Equals(ItemType.Weapon.ToString()) || type.Equals(ItemType.Shield.ToString()))
				{
					foreach (GameObject item in weaponsTiles3)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else //Equipment
				{
					foreach (GameObject item in equipmentTiles3)
					{
						if (item.GetComponent<Item>().itemName.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				break;
			case 4:
				if (type.Equals(ItemType.Consumable.ToString()))
				{
					foreach (GameObject item in consumibleTiles4)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else if (type.Equals(ItemType.Weapon.ToString()) || type.Equals(ItemType.Shield.ToString()))
				{
					foreach (GameObject item in weaponsTiles4)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else //Equipment
				{
					foreach (GameObject item in equipmentTiles4)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				break;
			case 5:
				if (type.Equals(ItemType.Consumable.ToString()))
				{
					foreach (GameObject item in consumibleTiles5)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else if (type.Equals(ItemType.Weapon.ToString()) || type.Equals(ItemType.Shield.ToString()))
				{
					foreach (GameObject item in weaponsTiles5)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				else //Equipment
				{
					foreach (GameObject item in equipmentTiles5)
					{
						if (item.name.Equals(name))
						{
							return Instantiate<GameObject>(item).GetComponent<Item>();
						}
					}
				}
				break;
			default:
				break;
		}
		return null;
	}
}
