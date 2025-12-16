using UnityEngine;

public enum ItemType
{
    Consumable,   
    Quest        
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
}
