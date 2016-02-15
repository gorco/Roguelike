using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerClickHandler {

	public Image img;
	public Item item;

	private bool specialized;
	private ItemType spezialitation;

	void Start()
	{
		this.img = this.transform.GetChild(0).GetComponentInChildren<Image>();
	}

	public void AddSpecialization(ItemType spezialitation)
	{
		this.spezialitation = spezialitation;
		this.specialized = true;
	}

	public ItemType GetSpecialization()
	{
		return this.spezialitation;
	}

	public bool IsSpecialized()
	{
		return this.specialized;
	}

	public void AddItem(Item item)
	{
		ChangeItem(item);
	}

	public Item GetCurrentItem()
	{
		return this.item;
	}

	public bool ChangeItem(Item newItem)
	{
		this.item = newItem;
		if (newItem != null)
		{
			if (!specialized || spezialitation == newItem.itemType)
			{
				if (this.img == null)
				{
					this.img = this.transform.GetChild(0).GetComponent<Image>();
				}
				this.img.enabled = true;
				this.img.sprite = newItem.itemSprite;
				return true;
			}
		}
		else
		{
			this.img.sprite = null;
			this.img.enabled = false;
		}

		return false;

	}

	public void HideItem(Boolean hide)
	{
		this.img.enabled = !hide;
	}

	public void RemoveItem()
	{
		this.item = null;
		this.img.enabled = false;
	}

	public bool IsEmpty()
	{
		return this.item == null;
	}

	public void UseItem()
	{
		if (!IsEmpty())
		{
			if (item.CanBeEquiped())
			{
				Inventory.Inv.EquipItem(this);
			} 
			else if (item.CanBeConsumed())
			{
				item.Use();
				Inventory.Inv.IncreaseEmptyCount();
				item = null;
				img.enabled = false;
			}
			else if (item.CanBeUsed())
			{

			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(eventData.button == PointerEventData.InputButton.Right)
		{
			UseItem();
		}
	}
}
