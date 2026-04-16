using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class QuickSlotHUDUI : MonoBehaviour
{
    public TMP_Text qty;
    public Image icon;

    public void UpdateSlot(Sprite itemIcon, int itemQty)
    {
        if (itemIcon == null || itemQty <= 0)
        {
            icon.enabled = false;
            icon.color = new Color32(255, 255, 255, 0);
            qty.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.color = new Color32(255, 255, 255, 255);
            icon.sprite = itemIcon;
            qty.text = itemQty.ToString();
        }
    }
}
