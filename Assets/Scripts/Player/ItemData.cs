using UnityEngine;
using data.structs;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string Description;

     public ItemType itemType;

    public bool isStackable;
    public int maxStack = 1;
    public Sprite icon;
}
