using System.Collections;
using data.structs;
using UnityEngine;
using Player.script;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MapIdentity : MonoBehaviour
{
    [Header("Map setting")]
    [SerializeField] public int mapLevel;
    [SerializeField] private int minEnemyLevel;
    [SerializeField] private int maxEnemyLevel;
    [SerializeField] public bool isSafeArea = false;
    [SerializeField] Vector2 FRONTLoc;
    [SerializeField] Vector2 ENDLoc;

    
    public static MapIdentity Instance;

    private int countEnemy = 0;
    public bool isCanCollectExp = false, isCanRestartGame;
    Transform playerT;

    public string lastSaveScene;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameEvents.CalculateEnemyStatByMapLevel?.Invoke(minEnemyLevel, maxEnemyLevel);
        GameObject[] enemiesArray = GameObject.FindGameObjectsWithTag("Enemy");
        countEnemy = enemiesArray.Length;

        // Check every enemy if thats nest
        foreach (GameObject en in enemiesArray)
        {
            ENestLarva eNestLarva = en.GetComponent<ENestLarva>();
            if (eNestLarva)
            {
                countEnemy += eNestLarva.spawnCount;
            }
        }
        
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
        GameEvents.MapClear?.Invoke();
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

            if(isSafeArea) lastSaveScene = SceneManager.GetActiveScene().name;
            else
            lastSaveScene = gameState.lastSaveScene ?? "map1";
        }
        else
        {
            Debug.Log("No save found or save corrupted.");
        }
    }

    void GenerateSaveDat()
    {
        // delete first
        SaveSceneManager.DeleteSaveFile();

        GameState gameState1 = new GameState();

        gameState1.level = 3;
        gameState1.currentScene = "map1";
        gameState1.lastSaveScene = "map1";
        gameState1.InventoryPlayer = new InventorySlot[30];
        // gameState1.weapon = new Weapon();
        gameState1.weapon = null;
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
        Debug.Log($"cuurent weapon player : {gs.weapon?.name ?? "Kosong"}");
        Debug.Log($"savemap : {gs.lastSaveScene}");
    }

    IEnumerator DelayInit(float d)
    {
        yield return new WaitForSeconds(d);
        // END Of loading
    }

    void PreparePlayerLoc()
    {
        if (PlayerPrefs.GetString("SpawnLoc", "FRONT") == "FRONT") PlayerHit.Instance.transform.position = FRONTLoc;
        // else PlayerHit.Instance.transform.position = ENDLoc;
        else PlayerHit.Instance.transform.position = FRONTLoc;
    }

    void PreparePlayerWeapon(Weapon? weapon)
    {
        PlayerHit.Instance.SetWeapon(weapon);
    }
}
