using UnityEngine;
using TMPro;
using UnityEngine.UI;
using data.structs;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;

    public void UpdateUI(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty())
        {
            icon.enabled = false;
            quantityText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slot.item.icon;

            // Tampilkan angka jika item lebih dari 1, kalau cuma 1 biasanya tidak perlu angka
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }
}
