using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using data.structs;
using Player.script;
using Unity.VisualScripting;
using System.Linq;
public class HUDUI : MonoBehaviour
{
    public static HUDUI Instance;

    public TMP_Text textLevel, textHP;
    public Image fillHealth, fillStamina, fillExp;
    public Image weaponIcon, armorIcon;

    public CanvasGroup canvasGroupSlotBar;
    public List<QuickSlotHUDUI> listQuickUIHUD;

    public float fadeDuration = 0.5f;
    public float displayDuration = 4f;

    public Animator animatorLevelUp;
    public Transform transformContentMessage;
    public GameObject PrefabNotifCollect;

    private Coroutine fadeCoroutine;

    int isShowQuickSlot = 0;

    void Awake() => Instance = this;

    void Start()
    {
        // Sembunyikan slot bar di awal
        canvasGroupSlotBar.alpha = 0;
        UpdateFillExp();
        UpdateFillStamina();
        UpdateFillHP();

        ClearQuickSlotBar();
        ShowQuickSlotBar();
    }

    void ClearQuickSlotBar()
    {
        foreach (var i in listQuickUIHUD)
        {
            i.qty.text = "";
            i.icon.color = new Color32(255, 255, 255, 0);
        }
    }

    public void UpdateQuickSlotHUD()
    {
        for (int i = 0; i < PlayerInventory.Instance.slotsQuickHUD.Length; i++)
        {
            var slot = listQuickUIHUD[i];
            var item = PlayerInventory.Instance.slotsQuickHUD[i];

            slot.qty.text = item.quantity > 0 ? item.quantity.ToString() : "";
            slot.icon.color = item != null && item.quantity > 0 ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
        }
    }

    void Update()
    {
        // Cek input Z, X, C, V (index 0, 1, 2, 3)
        if (Input.GetKeyDown(KeyCode.Z)) UseQuickSlot(3);
        if (Input.GetKeyDown(KeyCode.X)) UseQuickSlot(2);
        if (Input.GetKeyDown(KeyCode.C)) UseQuickSlot(1);
        if (Input.GetKeyDown(KeyCode.V)) UseQuickSlot(0);
    }

    public void UseQuickSlot(int index)
    {
        // Tampilkan UI
        ShowQuickSlotBar();

        // Logika Pakai Item (Misal: ambil dari list item konsumsi di inventory)
        if (isShowQuickSlot > 1)
            PlayerInventory.Instance.UseItemAt(index);

        Debug.Log($"Mencoba menggunakan item di slot {index}");

        // listQuickUIHUD[index].StartCoroutine(itemDropScript.StartDrop()); (itemToAssign.item.icon, itemToAssign.quantity);
        UpdateQuickSlotHUD();

        // Refresh UI setelah pakai
        UpdateHUD();
    }

    public void ShowQuickSlotBar()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine());
        isShowQuickSlot++;
    }

    IEnumerator FadeRoutine()
    {
        // Fade In
        canvasGroupSlotBar.DOFade(1f, fadeDuration);

        // Tunggu 3 detik
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        canvasGroupSlotBar.DOFade(0f, fadeDuration);
        isShowQuickSlot = 0;
    }

    public void UpdateFillHP()
    {
        PlayerStatus stats = PlayerStat.Instance.playerStatus;
        textHP.text = $"{stats.hp}/{stats.maxHP}";
        fillHealth.fillAmount = (float)stats.hp / stats.maxHP;
    }
    public void UpdateFillStamina()
    {
        PlayerStatus stats = PlayerStat.Instance.playerStatus;
        fillStamina.fillAmount = (float)stats.stamina / stats.maxStamina;
    }
    public void UpdateFillExp()
    {
        fillExp.fillAmount = (float)PlayerStat.Instance.expPlayer / PlayerStat.Instance.expRequiredForLevelUp;
        textLevel.text = "Level " + PlayerStat.Instance.level;
    }

    public void UpdateHUD()
    {
        // Update HP, Exp, Level dll sesuai data PlayerStat
        PlayerStatus stats = PlayerStat.Instance.playerStatus;
        textLevel.text = "Level " + PlayerStat.Instance.level;
        textHP.text = $"{stats.hp}/{stats.maxHP}";
        fillHealth.fillAmount = (float)stats.hp / stats.maxHP;

        // Update Icons
        var currentWeapon = PlayerHit.Instance.GetCurrentWeapon();
        Debug.Log($"ID :{currentWeapon.Value.ID}");
        if (currentWeapon != null) weaponIcon.sprite = PlayerHit.Instance.spritesOfWeapon[currentWeapon != null ? currentWeapon.Value.ID - 1 : 0];
    }

    public void AddNotifItem(Sprite img, string text)
    {
        GameObject g = Instantiate(PrefabNotifCollect, transformContentMessage);
        SingleNotifUI g_script = g.GetComponent<SingleNotifUI>();
        g_script.Init(img, text);
    }

    public void ShowNotifLevelUp()
    {
        animatorLevelUp.Play("start", 0, 0);
    }
}
