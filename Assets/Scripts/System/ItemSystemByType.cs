using UnityEngine;
using data.structs;
public class ItemSystemByType : MonoBehaviour
{
    public bool CanCraft(CraftRecipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            int total = CountItem(recipe.ingredients[i]);

            if (total < recipe.amounts[i])
                return false;
        }

        return true;
    }

    public int CountItem(ItemData item)
    {
        int total = 0;

        // foreach (var slot in slots)
        // {
        //     if (slot.item == item)
        //     {
        //         total += slot.quantity;
        //     }
        // }

        return total;
    }

    public bool Craft(CraftRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        // remove bahan
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            // RemoveItem(recipe.ingredients[i], recipe.amounts[i]);
        }

        // add hasil
        // AddItem(recipe.result, recipe.resultAmount);

        Debug.Log("Craft berhasil: " + recipe.result.itemName);

        return true;
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.IsEmpty()) return;

        switch (slot.item.itemType)
        {
            case ItemType.Consumable:
                Debug.Log("Pakai item: " + slot.item.itemName);

                // contoh: heal player
                // player.Heal(20);

                slot.quantity--;

                if (slot.quantity <= 0)
                    slot.Clear();

                break;

            case ItemType.Material:
                Debug.Log("Material tidak bisa dipakai langsung");
                break;

            case ItemType.Equipment:
                Debug.Log("Equip item");
                // nanti bikin equipment system
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
