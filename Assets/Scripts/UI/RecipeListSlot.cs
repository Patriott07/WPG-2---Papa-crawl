using UnityEngine;
using UnityEngine.UI;
using data.structs;
using UnityEngine.EventSystems;

public class RecipeListSlot : MonoBehaviour, IPointerDownHandler
{
    public Image itemIcon;
    public int index;
    private CraftRecipe recipe;

    void Start()
    {
        // foreach (var i in CraftingManager.Instance.database.allRecipes) Debug.Log(i.result.itemName);
        recipe = CraftingManager.Instance.database.allRecipes[index];
        Setup(recipe);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CraftingManager.Instance.DisplayRecipeDetail(recipe);
    }
    public void Setup(CraftRecipe r)
    {
        recipe = r;
        itemIcon.sprite = r.result.icon;
    }

    public void OnClickSlot()
    {
        // Beritahu UI Utama untuk menampilkan detail resep ini
        CraftingManager.Instance.DisplayRecipeDetail(recipe);
    }
}
