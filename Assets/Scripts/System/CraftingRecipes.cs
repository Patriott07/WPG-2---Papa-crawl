using UnityEngine;
using data.structs;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CraftingDatabase", menuName = "Inventory/CraftingDatabase")]
public class CraftingRecipes : ScriptableObject
{
    public List<CraftRecipe> allRecipes;
}
