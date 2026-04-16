using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using data.structs;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public CanvasGroup canvasGroupContent;
    bool isOpenInventory = false;
    public List<InventorySlotUI> slotsUI;

    [Header("UI Elements Inventory Detail")]
    public Image bigIcon;
    public Text textName;
    public Text textDescription;
    public Text textType;
    public Text textQty;
    public Text textStackable;
    private ItemData itemDataActive;

    [Header("Buttons")]
    public GameObject useButtonRight;
    public GameObject dropButtonRight;

    public static InventorySlot selectedItemForQuickSlot;


    void Start()
    {
        PlayerInventory.Instance.SortInventory();
        itemDataActive = PlayerInventory.Instance.slots[0].item ?? null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OpenCloseInventory();
    }

    public static void SetSelectedItemForQuickSlot(InventorySlot item)
    {
        bool isValid = item.item.itemType == ItemType.Consumable;
        if (isValid)
        {
            selectedItemForQuickSlot = item;
            Debug.Log("Sudah masuk ke quick slot");
        }
    }

    void OnEnable()
    {
        GameEvents.ChangeDetailOfInventoryItem += ChangeDetailInventoryItem;
    }
    void OnDisable()
    {
        GameEvents.ChangeDetailOfInventoryItem -= ChangeDetailInventoryItem;
    }

    void ChangeDetailInventoryItem(ItemData data)
    {
        if (data == null) return;

        itemDataActive = data;

        // 1. Set Teks dan Gambar
        // bigIcon.gameObject.SetActive(true);
        bigIcon.color = new Color32(255, 255, 255, 245);
        bigIcon.sprite = data.icon;
        textName.text = data.itemName;
        textDescription.text = data.Description;
        textType.text = data.itemType.ToString();

        // Info tambahan (Qty biasanya diambil dari slot, tapi ini contoh dari data)
        textQty.text = "Qty : 1";
        textStackable.text = "Stackable : " + (data.isStackable ? data.maxStack.ToString() : "1");

        // 2. Logika Munculin Tombol Use
        // Sesuai request: Hanya keluar klo Consumable, Equipment, atau Attachment
        bool canUse = data.itemType == ItemType.Consumable ||
                      data.itemType == ItemType.Equipment ||
                      data.itemType == ItemType.Attachment;

        useButtonRight.SetActive(canUse);
        // Tombol Drop biasanya selalu ada, kecuali item penting/Quest
        dropButtonRight.SetActive(data.itemType != ItemType.Quest);
    }

    public void DropItemInteract()
    {
        if (itemDataActive == null) return;
        PlayerInventory.Instance.RemoveItem(itemDataActive, 1);

        // AMBIL DATA DARI PLAYER INVENTORY
        InventorySlot[] playerSlots = PlayerInventory.Instance.slots;

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (i < playerSlots.Length)
            {
                // Kirim data slot ke UI berdasarkan index yang sama
                slotsUI[i].UpdateUI(playerSlots[i]);
                slotsUI[i].SetItemData(playerSlots[i].item);
            }
        }
    }

    void OpenCloseInventory()
    {
        isOpenInventory = !isOpenInventory;
        if (isOpenInventory)
        {
            // 1. RAPIKAN DATA TERLEBIH DAHULU
            PlayerInventory.Instance.SortInventory();

            canvasGroupContent.blocksRaycasts = true;
            canvasGroupContent.interactable = true;
            canvasGroupContent.DOFade(1f, 0.6f).OnComplete(() =>
            {
                // Gunakan Unscaled Time agar DOTween tetap jalan jika butuh, 
                // tapi hati-hati Time.timeScale = 0 mematikan semua update fisik.
                Time.timeScale = 0;
            });

            // AMBIL DATA DARI PLAYER INVENTORY
            InventorySlot[] playerSlots = PlayerInventory.Instance.slots;

            for (int i = 0; i < slotsUI.Count; i++)
            {
                if (i < playerSlots.Length)
                {
                    // Kirim data slot ke UI berdasarkan index yang sama
                    slotsUI[i].UpdateUI(playerSlots[i]);
                    slotsUI[i].SetItemData(playerSlots[i].item);
                }
            }
        }
        else
        {
            Time.timeScale = 1;
            canvasGroupContent.blocksRaycasts = false;
            canvasGroupContent.interactable = false;
            canvasGroupContent.DOFade(0f, 0.6f); // Fade ke 0 biar hilang
        }
    }


}
