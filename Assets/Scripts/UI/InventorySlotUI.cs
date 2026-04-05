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
        if (slot.IsEmpty())
        {
            icon.enabled = false;
            quantityText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slot.item.icon;
            quantityText.text = slot.quantity.ToString();
        }
    }
}
