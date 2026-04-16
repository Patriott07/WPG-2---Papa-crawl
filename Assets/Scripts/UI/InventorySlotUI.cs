using UnityEngine;
using TMPro;
using UnityEngine.UI;
using data.structs;
using UnityEngine.EventSystems;
public class InventorySlotUI : MonoBehaviour, IPointerDownHandler
{
    public Image icon;
    public TMP_Text quantityText;
    public ItemData itemData;
    InventorySlot _slot;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameEvents.ChangeDetailOfInventoryItem?.Invoke(itemData);
        InventoryUI.SetSelectedItemForQuickSlot(_slot);
    }

    public void UpdateUI(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty())
        {
            icon.enabled = false;
            if (quantityText != null)
                quantityText.text = "";
        }
        else
        {
            _slot = slot;
            icon.enabled = true;
            icon.sprite = slot.item.icon;

            // Tampilkan angka jika item lebih dari 1, kalau cuma 1 biasanya tidak perlu angka
            if (quantityText != null)
                quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;
        // gameObject.GetComponent<DragHandlerSlotInventory>().itemData = data;
    }
}
