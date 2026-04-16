using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class SingleNotifUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text name_item;
    public CanvasGroup canvasGroup;
    public float durationKill = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Init(Sprite image, string text)
    {
        canvasGroup.alpha = 0;
        icon.sprite = image;
        name_item.text = text;
        canvasGroup.DOFade(1, 0.4f);

        Destroy(gameObject, durationKill);
    }

}
