using UnityEngine;
using data.structs;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    public void Collect(PlayerInventory inventory)
    {
        bool success = inventory.AddItem(itemData, amount);

        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory penuh!");
        }
    }
}
