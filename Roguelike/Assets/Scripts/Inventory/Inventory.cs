using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public Slot slotPrefab;
	public int outPading = 5;
	public int slots = 25;
	public int rows = 5;
	public int leftPad = 2;
	public int topPad = 2;

	private float inventoryW, inventoryH;
	private List<Slot> slotsList = new List<Slot>();

	public Slot from, to;

	private int emptySlots = 0;

	public static Inventory Inv;

	void Start () {
		CreateInventoryLayout();

		Inv = this;
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
				from.GetComponent<Image>().color = Color.yellow;
			}
		}

		else if (to == null)
		{
			to = clicked.GetComponent<Slot>();
		}

		if(to!=null && from != null)
		{
			Item tmp = to.GetCurrentItem();
			to.ChangeItem(from.GetCurrentItem());
			from.ChangeItem(tmp);
			
			from.GetComponent<Image>().color = Color.white;

			to = null;
			from = null;
		}
	}
}
