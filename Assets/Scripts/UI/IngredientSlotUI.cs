using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class IngredientSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void Setup(ItemData item, int required, int owned)
    {
        gameObject.SetActive(true);
        icon.sprite = item.icon;
        // Warna merah jika bahan tidak cukup
        amountText.text = $"{owned}/{required}";
        amountText.color = owned >= required ? Color.white : Color.red;
    }

    public void Clear() => gameObject.SetActive(false);
}
