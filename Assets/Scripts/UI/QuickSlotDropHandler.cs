using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using data.structs;

public class QuickSlotDropHandler : MonoBehaviour, IPointerDownHandler
{
    public int slotIndex; // 0, 1, 2, 3
    public Image icon;
    public TMP_Text quantityText;
    public ItemData itemData;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnQuickSlotClicked();
    }

    void Awake()
    {
        icon.color = new Color32(255, 255, 255, 0);
    }

    public void OnQuickSlotClicked()
    {
        // Cek apakah ada item yang sudah dipilih dari inventory
        if (InventoryUI.selectedItemForQuickSlot != null)
        {
            InventorySlot itemToAssign = InventoryUI.selectedItemForQuickSlot;
            icon.sprite = itemToAssign.item.icon;
            icon.color = new Color32(255, 255, 255, 255);

            // 1. Update Visual HUD
            HUDUI.Instance.listQuickUIHUD[slotIndex].UpdateSlot(itemToAssign.item.icon, itemToAssign.quantity);
            PlayerInventory.Instance.AddItemQuickSlot(itemToAssign.item, itemToAssign.quantity);

            // 2. Reset selection agar tidak terpilih terus
            InventoryUI.selectedItemForQuickSlot = null;

            Debug.Log("Item berhasil dipasang ke QuickSlot " + (slotIndex + 1));
        }
        else
        {
            Debug.Log("Pilih item dulu di inventory baru pencet sini!");
        }
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
            icon.enabled = true;
            icon.sprite = slot.item.icon;

            // Tampilkan angka jika item lebih dari 1, kalau cuma 1 biasanya tidak perlu angka
            if (quantityText != null)
                quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    
}
