using UnityEngine;
using data.structs;

public class PlayerInventory : MonoBehaviour
{

    public int maxSlots = 30;
    public InventorySlot[] slots;

    public static PlayerInventory Instance;

    void Awake()
    {
        Instance = this;
        slots = new InventorySlot[maxSlots];

        for (int i = 0; i < maxSlots; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    // =========================
    // ADD ITEM
    // =========================
    public bool AddItem(ItemData item, int amount)
    {
        if (item.isStackable)
        {
            // 1. Isi slot yang sudah ada
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStack)
                {
                    int space = item.maxStack - slots[i].quantity;
                    int toAdd = Mathf.Min(space, amount);

                    slots[i].quantity += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                        return true;
                }
            }

            // 2. Isi slot kosong
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty())
                {
                    int toAdd = Mathf.Min(item.maxStack, amount);

                    slots[i].item = item;
                    slots[i].quantity = toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                        return true;
                }
            }
        }
        else
        {
            // item non-stack
            for (int j = 0; j < amount; j++)
            {
                bool placed = false;

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].IsEmpty())
                    {
                        slots[i].item = item;
                        slots[i].quantity = 1;
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                    return false; // inventory penuh
            }
        }

        return true;
    }

    // =========================
    // REMOVE ITEM
    // =========================
    public bool RemoveItem(ItemData item, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                if (slots[i].quantity >= amount)
                {
                    slots[i].quantity -= amount;

                    if (slots[i].quantity <= 0)
                        slots[i].Clear();

                    return true;
                }
                else
                {
                    amount -= slots[i].quantity;
                    slots[i].Clear();
                }
            }
        }

        return false;
    }
}
