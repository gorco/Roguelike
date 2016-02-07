﻿using UnityEngine;
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
	private List<Slot> slotsList = new List<Slot>();

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

	public static List<Slot> equipmentSlots = new List<Slot>(); 

	void Awake ()
	{
		Inv = this;

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
				slot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading + (leftPad + slotSize.x) * column, orgY - outPading - (topPad + slotSize.y) * row, 0);
				slotsList.Add(slot);
				emptySlots++;
			}
		}

		//Equipment Panel
		equipment = Instantiate(equipmentPrefab);
		equipment.transform.SetParent(this.transform);

		float equipmentW = slotSize.x * 2 + outPading * 5;

		RectTransform equipmentRect = equipment.GetComponent<RectTransform>();
		equipmentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, equipmentW);
		equipmentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryH);

		equipmentRect.localPosition = new Vector3(-inventoryW + equipmentW/2, 0,0);

		//InventoryBar
		inventoryBar = Instantiate(equipmentPrefab);
		inventoryBar.transform.SetParent(this.transform);
		float barW = equipmentW + inventoryW + leftPad;

		RectTransform barRect = inventoryBar.GetComponent<RectTransform>();
		barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barW);
		barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 36);

		barRect.localPosition = new Vector3(-(equipmentW+outPading)/2, (inventoryH + 36)/2, 0);

		close.transform.SetParent(inventoryBar.transform);
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
		helmetSlot.AddSpecialization(ItemType.Head);
		helmetSlot.transform.SetParent(equipment.transform);
		helmetSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading*2.5f + slotSize.x/2, orgY - outPading*2, 0);
		equipmentSlots.Add(helmetSlot);

		//Weapon slot
		Slot weaponSlot = Instantiate(slotPrefab);
		weaponSlot.AddSpecialization(ItemType.Weapon);
		weaponSlot.transform.SetParent(equipment.transform);
		weaponSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2, orgY - outPading * 2 - slotSize.y - outPading, 0);
		equipmentSlots.Add(weaponSlot);

		//Shield slot
		Slot shieldSlot = Instantiate(slotPrefab);
		shieldSlot.AddSpecialization(ItemType.Shield);
		shieldSlot.transform.SetParent(equipment.transform);
		shieldSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 3 + slotSize.x, orgY - outPading * 2 - slotSize.y - outPading, 0);
		equipmentSlots.Add(shieldSlot);

		//Armor slot
		Slot armorSlot = Instantiate(slotPrefab);
		armorSlot.AddSpecialization(ItemType.Armor);
		armorSlot.transform.SetParent(equipment.transform);
		armorSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2, orgY - outPading * 2 - (slotSize.y + outPading)*2, 0);
		equipmentSlots.Add(armorSlot);

		//Hands slot
		Slot hadsSlot = Instantiate(slotPrefab);
		hadsSlot.AddSpecialization(ItemType.Hands);
		hadsSlot.transform.SetParent(equipment.transform);
		hadsSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 3 + slotSize.x, orgY - outPading * 2 - (slotSize.y + outPading) * 2, 0);
		equipmentSlots.Add(hadsSlot);

		//Shoes slot
		Slot shoesSlot = Instantiate(slotPrefab);
		shoesSlot.AddSpecialization(ItemType.Shoes);
		shoesSlot.transform.SetParent(equipment.transform);
		shoesSlot.GetComponent<RectTransform>().localPosition = new Vector3(orgX + outPading * 2.5f + slotSize.x / 2, orgY - outPading * 2 - (slotSize.y + outPading) * 3, 0);
		equipmentSlots.Add(shoesSlot);

		this.transform.position = this.transform.position + new Vector3(outPading, 0, 0);
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			if (!eventSystem.IsPointerOverGameObject(-1) && from != null)
			{
				from.RemoveItem();
				Destroy(hoverObject);
				ResetInventoryState();
			}
		}

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
		
		if(this.enabled = true && !tmpSlot.IsEmpty() && hoverObject == null)
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
        } else if(!open && canvasGroup.alpha >= 0.5f)
		{
			canvasGroup.alpha = 0f;
		}
	}

	public void updateStatsText(string text)
	{
		statsText.text = text;
	}

	public void CalcStats()
	{
		Debug.Log("Calc STaTS");
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
}
