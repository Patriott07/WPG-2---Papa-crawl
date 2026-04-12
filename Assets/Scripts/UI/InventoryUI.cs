using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using data.structs;
public class InventoryUI : MonoBehaviour
{
    public CanvasGroup canvasGroupContent;
    bool isOpenInventory = false;
    public List<InventorySlotUI> slotsUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 // 1. RAPIKAN DATA TERLEBIH DAHULU
            PlayerInventory.Instance.SortInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OpenCloseInventory();
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
