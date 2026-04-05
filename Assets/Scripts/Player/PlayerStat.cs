using UnityEngine;
using data.structs;
using System.Collections;
using Unity.VisualScripting;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatus playerStatus, baseInit;
    public static PlayerStat Instance;
    bool canHit = true;
    public int level = 1;
    public float expPlayer;
    public float expRequiredForLevelUp;
    void Awake()
    {
        Instance = this;
        playerStatus = new PlayerStatus(30, 5, 6f, 5, 30);
        baseInit = playerStatus;
    }

    void Update()
    {
        CheckLevelUpPlayer();
    }

    void CheckLevelUpPlayer()
    {
        if (expPlayer >= expRequiredForLevelUp)
        {
            expPlayer -= expRequiredForLevelUp;
            float lastMaxHp = playerStatus.maxHP;

            level++;
            expRequiredForLevelUp = 150 * (level + 1) * 1.5f; // Requirement exp for next level up

            playerStatus.maxHP = baseInit.maxHP + (level * 20);
            playerStatus.hp += playerStatus.maxHP - lastMaxHp;
            playerStatus.attackPoint = baseInit.attackPoint + (level * 3);
            playerStatus.critDamage = baseInit.critDamage + (level * 0.01f);

            Debug.Log("Player Level Up");
        }
    }

    public void GainExp(float v)
    {
        expPlayer += v;
    }

    // calculation base level
    void CalculateBaseStatByLevel(GameState gs)
    {
        playerStatus.maxHP = baseInit.maxHP + (gs.level * 20);
        playerStatus.attackPoint = baseInit.attackPoint + (gs.level * 3);
        playerStatus.critDamage = baseInit.critDamage + (gs.level * 0.01f);

        level = gs.level;
        expPlayer = gs.currentExp;

        expRequiredForLevelUp = 150 * (level + 1) * 1.5f;

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

        if (playerStatus.hp <= 0) Dead();
    }

    void ShowNumberText(float damage)
    {
        GameObject text = ObjectPollingGame.Instance.GetUITextFloat();
        // set color, text, pos
        text.transform.position = new Vector2(transform.position.x + Random.Range(-0.8f, 0.8f), transform.position.y + 1.2f);
        TextUIFloatingDamage scriptText = text.GetComponent<TextUIFloatingDamage>();
        scriptText.colorText = Color.red;
        scriptText.textInput = damage.ToString();
        text.SetActive(true);

        scriptText.textTMP.rectTransform.sizeDelta = new Vector2(200, 65);
        scriptText.textTMP.fontStyle = TMPro.FontStyles.Bold;
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
    }

}

