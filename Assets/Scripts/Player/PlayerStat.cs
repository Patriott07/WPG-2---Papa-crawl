using UnityEngine;
using data.structs;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;
using Player.script;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatus playerStatus, baseInit;
    public static PlayerStat Instance;
    bool canHit = true;
    public int level = 1;
    public float expPlayer;
    public float expRequiredForLevelUp;
    public bool isAlive = true;
    void Awake()
    {
        Instance = this;
        playerStatus = new PlayerStatus(30, 5, 6f, 5, 30);
        baseInit = playerStatus;
    }

    void Update()
    {
        CheckLevelUpPlayer();
        if (Input.GetKeyDown(KeyCode.Alpha9))
            Dead();
    }

    void CheckLevelUpPlayer()
    {
        if (expPlayer >= expRequiredForLevelUp)
        {
            expPlayer -= expRequiredForLevelUp;
            float lastMaxHp = playerStatus.maxHP;

            level++;
            expRequiredForLevelUp = 150 * (level + 1) * 1.5f; // Requirement exp for next level up

            // play sound
            StartCoroutine(PlayerSounds.Instance.PlayDelayBeforeSound(0.1f, PlayerSounds.Instance.levelup));
            // ShowLevelUpText();
            HUDUI.Instance.ShowNotifLevelUp();

            playerStatus.maxHP = baseInit.maxHP + (level * 20);
            playerStatus.hp += playerStatus.maxHP - lastMaxHp;
            playerStatus.attackPoint = baseInit.attackPoint + (level * 3);
            playerStatus.critDamage = baseInit.critDamage + (level * 0.01f);

            HUDUI.Instance.UpdateFillExp();
            HUDUI.Instance.UpdateFillHP();
            HUDUI.Instance.UpdateHUD();

            Debug.Log("Player Level Up");
        }
    }

    public void GainHP(float v)
    {
        playerStatus.hp += v;
        if(playerStatus.hp >= playerStatus.maxHP) playerStatus.hp = playerStatus.maxHP;
    }

    public void GainExp(float v)
    {
        expPlayer += v;
    }

    // calculation base level
    void CalculateBaseStatByLevel(GameState gs)
    {
        playerStatus.maxHP = baseInit.maxHP + (gs.level * 20);
        playerStatus.hp = gs.player.hp;
        playerStatus.attackPoint = baseInit.attackPoint + (gs.level * 3);
        playerStatus.critDamage = baseInit.critDamage + (gs.level * 0.01f);

        level = gs.level;
        expPlayer = gs.currentExp;

        expRequiredForLevelUp = 150 * (level + 1) * 1.5f;

        HUDUI.Instance.UpdateFillHP();
        HUDUI.Instance.UpdateFillExp();
        HUDUI.Instance.UpdateHUD();

        Debug.Log($"Sudah dihitung  Level Player : {level}, {expPlayer}, need {expRequiredForLevelUp} for level up");
    }


    public void setLevel(int s) { level = s; }
    public int getLevel() => level;

    void GetDamage(float damage)
    {
        if (!canHit) return;
        canHit = false;
        StartCoroutine(DelayBeforeCanGetHitAgain());
        ShowNumberText(damage);

        playerStatus.hp -= damage;
        Debug.Log($"Player getdamage by enemy : {damage}");

        Camera.main.DOShakePosition(0.08f, 0.3f);
        HUDUI.Instance.UpdateFillHP();

        if (playerStatus.hp <= 0) Dead();
    }

    void ShowLevelUpText()
    {
        GameObject text = ObjectPollingGame.Instance.GetUITextFloat();
        // set color, text, pos
        text.transform.position = new Vector2(transform.position.x + Random.Range(-0.4f, 0.4f), transform.position.y + 0.6f);
        TextUIFloatingDamage scriptText = text.GetComponent<TextUIFloatingDamage>();
        scriptText.colorText = Color.yellow;
        scriptText.textInput = "LEVEL UPP++";

        scriptText.textTMP.rectTransform.sizeDelta = new Vector2(200, 65);
        scriptText.textTMP.fontStyle = TMPro.FontStyles.Bold;
        scriptText.dDestroy = 2f;
        text.SetActive(true);

    }

    void ShowNumberText(float damage)
    {
        GameObject text = ObjectPollingGame.Instance.GetUITextFloat();
        // set color, text, pos
        text.transform.position = new Vector2(transform.position.x + Random.Range(-0.8f, 0.8f), transform.position.y + 1.2f);
        TextUIFloatingDamage scriptText = text.GetComponent<TextUIFloatingDamage>();
        scriptText.colorText = Color.red;
        scriptText.textInput = damage.ToString();

        scriptText.textTMP.rectTransform.sizeDelta = new Vector2(200, 65);
        scriptText.textTMP.fontStyle = TMPro.FontStyles.Bold;

        text.SetActive(true);
    }

    IEnumerator DelayBeforeCanGetHitAgain()
    {
        yield return new WaitForSeconds(0.4f);
        canHit = true;
    }

    void OnEnable()
    {
        GameEvents.OnPlayerGetDamage += GetDamage;
        GameEvents.SaveManagerLoaded += CalculateBaseStatByLevel;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerGetDamage -= GetDamage;
        GameEvents.SaveManagerLoaded -= CalculateBaseStatByLevel;
    }

    void Dead()
    {
        Debug.Log("Player Deadd");
        if (!isAlive) return;
        playerStatus.hp = 0;
        isAlive = false;
        canHit = false;
        PlayerHit.Instance.SetCanShoot(false);
        PlayerMovement.Instance.SetCanMove(false);
        GameEvents.OnPlayerDead?.Invoke();
    }

}

