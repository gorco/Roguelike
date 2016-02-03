using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerClickHandler {

	private Image img;
	private Item item;

	void Start()
	{
		img = this.transform.GetChild(0).GetComponentInChildren<Image>();
	}

	public void AddItem(Item item)
	{
		ChangeItem(item);
	}

	public Item GetCurrentItem()
	{
		return this.item;
	}

	public void ChangeItem(Item newItem)
	{
		this.item = newItem;
		if(newItem != null)
		{
			img.sprite = item.itemSprite;
			img.enabled = true;
			return;
		}
		img.sprite = null;
		img.enabled = false;

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
			if (item.Use())
			{
				Inventory.Inv.IncreaseEmptyCount();
				item = null;
				img.enabled = false;
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
