using System.Collections.Generic;
using data.structs;
using UnityEngine;

public class ModelsData : MonoBehaviour
{
    public static List<Weapon> weaponsModel1 = new List<Weapon>
        {
            new Weapon(1,"Slingshot", 25, 15, 5, 5f,2, 6, -5.1f, 10, -45f),
            new Weapon(2,"Bow", 35, 20, 9, 5f, 3, -0.5f, -5.6f, 0, 45f),
            new Weapon(3, "Dagger", 20, 5, 7, 7, 2,4.9f,-4.8f, 0, -45f),
            new Weapon(4, "Crossbow", 40, 20, 13, 6,3,-1f, -6.3f, 0, 0),
            // Weapon(int ID, string name, int speed, float damage, 
            // float range, float lifetime, int maxAttachment, 
            // float movementImpact, float attackSpeedImpact, float knockbackStrength)
        };
    public static List<Weapon> weaponsModel2;
    public static List<Weapon> weaponsModel3;
    public static List<Weapon> GetListOfWeapon1() => weaponsModel1;

    public static ModelsData Instance;


    void Awake()
    {
        Instance = this;

    }
}
