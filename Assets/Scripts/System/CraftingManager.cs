using UnityEngine;
using System.Collections.Generic;
using data.structs;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using DG.Tweening;


public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    public CanvasGroup canvasGroupContent;
    bool isOpenCrafting = false;

    [Header("Data")]
    public CraftingRecipes database; // Masukkan file .asset database kamu ke sini
    public List<RecipeListSlot> manualSlots;

    // [Header("List Left")]
    // public Transform listParent;
    // public GameObject recipeSlotPrefab;

    [Header("Detail Center")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text levelReqText;
    public Image bigPreviewIcon;

    [Header("Ingredients Right")]
    public IngredientSlotUI[] ingredientSlots;
    public Button craftButton;

    private CraftRecipe selectedRecipe;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvasGroupContent.alpha = 0;
        canvasGroupContent.blocksRaycasts = false;
        canvasGroupContent.interactable = false;
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.G))
        //     OpenClosePanel();
    }

    public bool CanCraft(CraftRecipe recipe)
    {
        // 1. Cek Level Requirement
        int playerLevel = PlayerStat.Instance.getLevel(); // Mengambil level dari script PlayerStat kamu
        if (playerLevel < recipe.levelRequired)
        {
            Debug.LogWarning($"Level tidak cukup! Butuh level {recipe.levelRequired}");
            return false;
        }

        // 2. Cek Apakah Bahan di Inventory Cukup
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            if (!HasIngredient(recipe.ingredients[i], recipe.amounts[i]))
            {
                Debug.LogWarning($"Bahan {recipe.ingredients[i].itemName} tidak cukup!");
                return false;
            }
        }

        return true;
    }

    public void CraftItem(CraftRecipe recipe)
    {
        if (CanCraft(recipe))
        {
            // 1. Kurangi bahan dari inventory
            for (int i = 0; i < recipe.ingredients.Length; i++)
            {
                PlayerInventory.Instance.RemoveItem(recipe.ingredients[i], recipe.amounts[i]);
            }

            // 2. Tambahkan hasil crafting ke inventory
            PlayerInventory.Instance.AddItem(recipe.result, recipe.resultAmount);

            Debug.Log($"Berhasil membuat {recipe.result.itemName}!");
        }
    }

    private bool HasIngredient(ItemData item, int requiredAmount)
    {
        int count = 0;
        foreach (var slot in PlayerInventory.Instance.slots)
        {
            if (slot.item == item)
            {
                count += slot.quantity;
            }
        }
        return count >= requiredAmount;
    }

    void OnEnable() => UpdateManualList();

    public void UpdateManualList()
    {
        // Ambil data dari database
        var allRecipes = database.allRecipes;

        for (int i = 0; i < manualSlots.Count; i++)
        {
            if (i < allRecipes.Count)
            {
                manualSlots[i].gameObject.SetActive(true);
                manualSlots[i].Setup(allRecipes[i]);
            }
            else
            {
                // Jika slot di UI lebih banyak dari jumlah resep di database, sembunyikan sisanya
                manualSlots[i].gameObject.SetActive(false);
            }
        }
    }

    // public void InitList()
    // {
    //     // Hapus list lama & ambil dari database
    //     foreach (Transform t in listParent) Destroy(t.gameObject);

    //     foreach (var r in database.allRecipes)
    //     {
    //         var slot = Instantiate(recipeSlotPrefab, listParent).GetComponent<RecipeListSlot>();
    //         slot.Setup(r);
    //     }
    // }

    public void DisplayRecipeDetail(CraftRecipe recipe)
    {
        selectedRecipe = recipe;
        titleText.text = recipe.result.itemName;
        descriptionText.text = "Crafting item ini untuk meningkatkan efisiensi bisnis!";
        levelReqText.text = $"Unlocked at lvl {recipe.levelRequired}";
        bigPreviewIcon.sprite = recipe.result.icon;

        // Update Slot Bahan
        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (i < recipe.ingredients.Length)
            {
                int owned = GetOwnedAmount(recipe.ingredients[i]);
                ingredientSlots[i].Setup(recipe.ingredients[i], recipe.amounts[i], owned);
            }
            else ingredientSlots[i].Clear();
        }

        // Cek apakah tombol Craft bisa ditekan
        craftButton.interactable = CanCraft(recipe);
    }

    int GetOwnedAmount(ItemData item)
    {
        int count = 0;
        foreach (var s in PlayerInventory.Instance.slots)
            if (s.item == item) count += s.quantity;
        return count;
    }

    public void PressCraft()
    {
        Debug.Log("TRY CRAFTING");
        if (selectedRecipe != null)
        {
            CraftItem(selectedRecipe);
            DisplayRecipeDetail(selectedRecipe); // Refresh tampilan setelah craft
        }
    }

    public void OpenClosePanel()
    {
        isOpenCrafting = !isOpenCrafting;
        if (isOpenCrafting)
        {
            DisplayRecipeDetail(database.allRecipes[0]);
            canvasGroupContent.blocksRaycasts = true;
            canvasGroupContent.interactable = true;
            canvasGroupContent.DOFade(1f, 0.6f).OnComplete(() =>
            {
                // Gunakan Unscaled Time agar DOTween tetap jalan jika butuh, 
                // tapi hati-hati Time.timeScale = 0 mematikan semua update fisik.
                Time.timeScale = 0;
            });

        }
        else
        {
            Time.timeScale = 1;
            canvasGroupContent.blocksRaycasts = false;
            canvasGroupContent.interactable = false;
            canvasGroupContent.DOFade(0f, 0.6f); // Fade ke 0 biar hilang
        }
    }
}
