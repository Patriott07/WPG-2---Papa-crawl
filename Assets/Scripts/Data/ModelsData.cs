using System.Collections.Generic;
using data.structs;
using UnityEngine;

public class ModelsData : MonoBehaviour
{
    public static List<Weapon> weaponsModel1;
    public static List<Weapon> weaponsModel2;
    public static List<Weapon> weaponsModel3;
    public static List<Weapon> GetListOfWeapon() => weaponsModel1;

    void Awake()
    {
        weaponsModel1 = new List<Weapon>
        {
            new Weapon(1,"Slingshot", 25, 34, 5, 5f,2, 6, -4.8f, 10),
            new Weapon(2,"Bow", 35, 10, 27, 5f, 3, -0.5f, -5.5f, 0),
            new Weapon(3, "Dagger", 20, 5, 16, 7, 2,6,-3.8f, 0),
            new Weapon(4, "Crossbow", 40, 20, 47, 6,3,-1f, -6, 0),
            // Weapon(int ID, string name, int speed, float damage, 
            // float range, float lifetime, int maxAttachment, 
            // float movementImpact, float attackSpeedImpact, float knockbackStrength)
        };
    }
}
