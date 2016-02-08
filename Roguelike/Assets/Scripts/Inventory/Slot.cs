﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerClickHandler {

	private Image img;
	private Item item;

	private bool specialized;
	private ItemType spezialitation;

	void Start()
	{
		img = this.transform.GetChild(0).GetComponentInChildren<Image>();
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
				img.sprite = item.itemSprite;
				img.enabled = true;
				return true;
			}
		}

		else
		{
			img.sprite = null;
			img.enabled = false;
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
		img.enabled = false;
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
