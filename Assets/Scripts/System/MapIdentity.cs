using System.Collections;
using data.structs;
using UnityEngine;
using Player.script;
using System.Collections.Generic;
public class MapIdentity : MonoBehaviour
{
    [Header("Map setting")]
    [SerializeField] private int mapLevel;
    [SerializeField] private int minEnemyLevel;
    [SerializeField] private int maxEnemyLevel;
    [SerializeField] private bool isSafeArea = false;
    [SerializeField] Vector2 FRONTLoc;
    [SerializeField] Vector2 ENDLoc;

    public static MapIdentity Instance;

    private int countEnemy = 0;
    public bool isCanCollectExp = false;
    Transform playerT;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameEvents.CalculateEnemyStatByMapLevel?.Invoke(minEnemyLevel, maxEnemyLevel);
        GameObject[] enemiesArray = GameObject.FindGameObjectsWithTag("Enemy");
        countEnemy = enemiesArray.Length;
        playerT = PlayerStat.Instance.transform;
    }

    public void DecreaseEnemyCount()
    {
        countEnemy--;
        if (countEnemy <= 0)
        {
            isCanCollectExp = true;
            StartCoroutine(StartCollectingExp());
        }
    }

    IEnumerator StartCollectingExp()
    {
        yield return new WaitForSeconds(0.6f);
        GameEvents.GetCollideWithPlayer?.Invoke(playerT);
    }

    public void SpawnObjectExp(Transform t, float AffectExp)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject exp = ObjectPollingGame.Instance.GetParticleExp();

            // spawn tepat di posisi enemy
            exp.transform.position = t.position;

            ExpParticle expScript = exp.GetComponent<ExpParticle>();
            expScript.expAffect = AffectExp;

            expScript.Init(t.position);

            exp.SetActive(true);
        }
    }

    void OnEnable()
    {
        GameEvents.LevelStart += Init;
    }

    void OnDisable()
    {
        GameEvents.LevelStart -= Init;
    }
    void Init(float d)
    {
        // Load state
        // GenerateSaveDat();
        LoadState();

        // Debug.Log()
        // if (isSafeArea) return;
        PreparePlayerLoc();
        StartCoroutine(DelayInit(d));
    }

    void LoadState()
    {
        GameState gameState = SaveSceneManager.Load();
        DebugIsiSaveData(gameState);

        PlayerPrefs.SetInt("levelMap", mapLevel);

        GameEvents.SaveManagerLoaded?.Invoke(gameState);

        if (gameState != null)
        {
            Debug.Log("=== SAVE LOADED ===");
            // prepare all of logic in item
            Debug.Log(JsonUtility.ToJson(gameState, true));
            PreparePlayerWeapon(gameState.weapon);
            PlayerStat.Instance.setLevel(gameState.level);
            PlayerInventory.Instance.slots = gameState.InventoryPlayer;
            PlayerStat.Instance.playerStatus = gameState.player;
        }
        else
        {
            Debug.Log("No save found or save corrupted.");
        }
    }

    void GenerateSaveDat()
    {
        GameState gameState1 = new GameState();

        gameState1.level = 3;
        gameState1.currentScene = "map1";
        gameState1.InventoryPlayer = new InventorySlot[30];
        gameState1.weapon = new Weapon();
        gameState1.player = new PlayerStatus(30, 5, 6f, 5, 30);
        gameState1.currentExp = 0;

        SaveSceneManager.Save(gameState1);
    }

    void DebugIsiSaveData(GameState gs)
    {
        Debug.Log("====INFO====");
        Debug.Log($"cuurent scene : {gs.currentScene}");
        Debug.Log($"cuurent exp player : {gs.currentExp}");
        Debug.Log($"cuurent level player : {gs.level}");
        Debug.Log($"cuurent level player : {gs.weapon.name}");
    }

    IEnumerator DelayInit(float d)
    {
        yield return new WaitForSeconds(d);
        // END Of loading
    }

    void PreparePlayerLoc()
    {
        if (PlayerPrefs.GetString("SpawnLoc", "FRONT") == "FRONT") PlayerHit.Instance.transform.position = FRONTLoc;
        else PlayerHit.Instance.transform.position = ENDLoc;
    }

    void PreparePlayerWeapon(Weapon weapon)
    {
        PlayerHit.Instance.SetWeapon(weapon);
    }
}
