using UnityEngine;
using data.structs;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Player.script;
using System.Collections;


public class PlayerInventory : MonoBehaviour
{

    public int maxSlots = 30;
    public InventorySlot[] slots;

    public List<ItemData> listItemTestWanInsert;

    public static PlayerInventory Instance;

    void Awake()
    {
        Instance = this;
        slots = new InventorySlot[maxSlots];

        for (int i = 0; i < maxSlots; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            foreach (var el in listItemTestWanInsert)
            {
                Debug.Log($"Test Element Equipment {el.itemName}");
                AddItem(el, 1);
            }
        }
    }

    // =========================
    // ADD ITEM
    // =========================
    public bool AddItem(ItemData item, int amount)
    {
        if (item.isStackable)
        {
            // 1. Isi slot yang sudah ada
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStack)
                {
                    int space = item.maxStack - slots[i].quantity;
                    int toAdd = Mathf.Min(space, amount);

                    slots[i].quantity += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                        return true;
                }
            }

            // 2. Isi slot kosong
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty())
                {
                    int toAdd = Mathf.Min(item.maxStack, amount);

                    slots[i].item = item;
                    slots[i].quantity = toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                        return true;
                }
            }
        }
        else
        {
            // item non-stack
            for (int j = 0; j < amount; j++)
            {
                bool placed = false;

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].IsEmpty())
                    {
                        slots[i].item = item;
                        slots[i].quantity = 1;
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                    return false; // inventory penuh
            }
        }

        return true;
    }

    // =========================
    // REMOVE ITEM
    // =========================
    public bool RemoveItem(ItemData item, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                if (slots[i].quantity >= amount)
                {
                    slots[i].quantity -= amount;

                    if (slots[i].quantity <= 0)
                        slots[i].Clear();

                    return true;
                }
                else
                {
                    amount -= slots[i].quantity;
                    slots[i].Clear();
                }
            }
        }

        return false;
    }

    public void SortInventory()
    {
        List<InventorySlot> occupiedSlots = new List<InventorySlot>();
        List<InventorySlot> emptySlots = new List<InventorySlot>();

        // 1. Pisahkan mana yang isi dan mana yang kosong
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty())
                occupiedSlots.Add(slot);
            else
                emptySlots.Add(slot);
        }

        // 2. Gabungkan kembali: Yang isi di depan, yang kosong di belakang
        int index = 0;
        foreach (var slot in occupiedSlots)
        {
            slots[index] = slot;
            index++;
        }
        foreach (var slot in emptySlots)
        {
            slots[index] = slot;
            index++;
        }

        Debug.Log("Inventory sorted!");
    }

    void ClearInventoryWhileDied()
    {
        foreach (InventorySlot el in slots)
        {
            // TAMBAHKAN PENGECEKAN INI:
            // Cek apakah slot null atau isinya null agar tidak error
            if (el == null || el.item == null) continue;

            if (el.item.itemType == ItemType.Equipment)
            {
                el.Clear();
            }
            else if (el.item.itemType == ItemType.Consumable)
            {
                el.Clear();
            }

        }

        // call saving data
        StartCoroutine(SaveAndRespawn());
    }

    IEnumerator SaveAndRespawn()
    {
        yield return new WaitForSeconds(2f);
        SaveSceneManager.Save(PrepareGameState(SceneManager.GetActiveScene().name));
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(MapIdentity.Instance.lastSaveScene == null || MapIdentity.Instance.lastSaveScene == "" ? "map1" : MapIdentity.Instance.lastSaveScene);
    }

    public GameState PrepareGameState(string currentMap)
    {
        GameState gameState = new GameState();
        PlayerStatus newStatForRespawn = PlayerStat.Instance.playerStatus;
        newStatForRespawn.hp = newStatForRespawn.maxHP;
        newStatForRespawn.stamina = newStatForRespawn.maxStamina;

        gameState.InventoryPlayer = slots;

        // gameState.craftedItem = new List<Item>();
        gameState.craftedItem = new List<Item>();
        gameState.player = newStatForRespawn;
        gameState.weapon = null;
        gameState.level = PlayerStat.Instance.getLevel();
        gameState.currentExp = PlayerStat.Instance.expPlayer;
        gameState.currentScene = currentMap;

        if (MapIdentity.Instance.isSafeArea) gameState.lastSaveScene = SceneManager.GetActiveScene().name;
        else gameState.lastSaveScene = MapIdentity.Instance.lastSaveScene;

        return gameState;
    }

    void OnEnable()
    {
        GameEvents.OnPlayerDead += ClearInventoryWhileDied;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDead -= ClearInventoryWhileDied;
    }
}
