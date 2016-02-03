using UnityEngine;

public enum ItemType
{
	Head,
	Armor,
	Hands,
	Shoes,
	Weapon,
	Shield,
	Consumable
}

public class Item : MonoBehaviour {

	public Sprite itemSprite;
	public ItemType itemType;
	public string itemName;
	public string itemInfo;
	public int power;
	public int value;

	public bool Use ()
	{
		Debug.Log("UsingItem");
		switch (itemType)
		{
			case ItemType.Consumable:
				break;
			default:
				break;
		}

		return true;
	}

}
