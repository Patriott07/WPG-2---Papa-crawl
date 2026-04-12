using System.Collections;
using System.Collections.Generic;
using data.structs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player.script;
using System.Linq;

public class TriggerChangeMap : MonoBehaviour
{
    public enum SpawnLoc { FRONT, END };
    // public bool isStart = true;
    public string nextSceneName;
    public SpawnLoc spawnLoc = SpawnLoc.FRONT;

    // bool isHasChangeScene = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeMap();
        }
    }

    void ChangeMap()
    {
        // get current
        PlayerPrefs.SetString("SpawnLoc", spawnLoc.ToString());
        // int mapLevel = PlayerPrefs.GetInt("levelMap", 1);
        int mapLevel = MapIdentity.Instance.mapLevel;
        LoadNextLevel(1f, mapLevel);
    }


    void LoadNextLevel(float loadingDur, int currentLevel)
    {
        Debug.Log("paanggil akuu..");

        // save here
        if (spawnLoc == SpawnLoc.FRONT)
            SaveSceneManager.Save(PrepareGameState($"map{currentLevel + 1}"));
        else
            SaveSceneManager.Save(PrepareGameState($"map{currentLevel - 1}"));

        StartCoroutine(LoadingUI.Instance.HideForX(loadingDur, () =>
        {
            if (spawnLoc == SpawnLoc.FRONT)
                SceneManager.LoadScene($"map{currentLevel + 1}"); // is should be last
            else
                SceneManager.LoadScene($"map{currentLevel - 1}"); // is should be last
        }));
    }

    public GameState PrepareGameState(string currentMap)
    {
        GameState gameState = new GameState();

        gameState.InventoryPlayer = PlayerInventory.Instance.slots;
        gameState.craftedItem = new List<Item>();
        gameState.player = PlayerStat.Instance.playerStatus;
        gameState.weapon = PlayerHit.Instance.GetCurrentWeapon();
        gameState.level = PlayerStat.Instance.getLevel();
        gameState.currentExp = PlayerStat.Instance.expPlayer;
        gameState.currentScene = currentMap;

        if (MapIdentity.Instance.isSafeArea) gameState.lastSaveScene = SceneManager.GetActiveScene().name;
        else gameState.lastSaveScene = MapIdentity.Instance.lastSaveScene;

        return gameState;
    }
}
