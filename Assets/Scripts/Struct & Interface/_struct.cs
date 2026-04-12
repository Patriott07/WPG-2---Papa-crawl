using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace data.structs
{
    public enum TypeOfItem { just_item, use_item, equipment };
    public enum TypeEnemy { Range, Mele, Tank };
    public enum TypeMovementEnemy { Chasing, NotMove };

    public enum ItemType
    {
        Consumable,   // bisa dipakai (potion, food)
        Material,     // bahan crafting
        Equipment,    // senjata, armor
        Attachment,    // attachment
        Quest,         // item quest
    }

    [System.Serializable]
    public class CraftRecipe
    {
        public ItemData result;
        public int resultAmount;

        public int levelRequired; // Level minimal untuk craft item ini

        public ItemData[] ingredients;
        public int[] amounts;
    }

    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;

        public bool IsEmpty()
        {
            return item == null;
        }

        public void Clear()
        {
            item = null;
            quantity = 0;
        }
    }

    public enum Weapons
    {
        Slingshot, Dagger, Bow, Crossbow
    }
    public enum AttachmentWeaponType
    {
        Damage,
        AttackSpeed,
        CriticalDamage,
        CriticalRate,
        Lifesteal,
        Penetration,
        Range,
    }

    [System.Serializable]
    public struct Weapon
    {
        public int ID;
        public string name;
        public float speed; // kecepatan peluru
        public float damage; // damage
        public float range; // jangkauan
        public float lifetime; // 
        public int penetration; // penet
        public int level;
        public float knockbackStrength;
        public float movementImpact;
        public float attackSpeedImpact;
        public int maxAttachment; // max attachemnt

        public float offsetAngleView;
        // public Weapons attachmentWeapon; // gunanya apa?

        public List<AttachmentWeaponType> attachments;
        public Weapon(int ID, string name, int speed, float damage, float range, float lifetime, int maxAttachment, float movementImpact, float attackSpeedImpact, float knockbackStrength,float angleView)
        {
            this.ID = ID;
            this.name = name;
            this.speed = speed;
            this.damage = damage;
            this.range = range;
            this.lifetime = lifetime;
            this.level = 1;
            this.penetration = 0;
            this.attachments = new List<AttachmentWeaponType>();
            // this.attachmentWeapon = attachmentWeapon;
            this.maxAttachment = maxAttachment;
            this.movementImpact = movementImpact;
            this.attackSpeedImpact = attackSpeedImpact;
            this.knockbackStrength = knockbackStrength;
            this.offsetAngleView = angleView;
        }
    }

    [System.Serializable]
    public struct CollectItem
    {
        public int ID;
        public string name;
        public int qty;
        public int maxStack;

        // public RawImage artUI;
        // public Sprite artSprite;

        public CollectItem(int ID, string name, int maxStack)
        {
            this.ID = ID;
            this.name = name;
            this.qty = 1;
            this.maxStack = maxStack;
            // this.artUI = artUI;
            // this.artSprite = artSprite;
        }
    }

    [System.Serializable]
    public struct Item // crafted item
    {
        public string name;
        public int qty;
        public string desc;
        public int maxStack;
        public TypeOfItem typeOfItem;

        public Item(string name, string desc, int maxStack, TypeOfItem typeOfItem)
        {
            this.name = name;
            this.desc = desc;
            this.qty = 1;
            this.maxStack = maxStack;
            this.typeOfItem = typeOfItem;
        }

    }

    [System.Serializable]
    public struct PlayerStatus
    {
        public float hp;
        public float maxHP;
        public int armor;
        public float movespeed;
        public float attackSpeed;
        public float attackPoint; // base 
        public float stamina;
        public float maxStamina;
        public float critDamage;

        public PlayerStatus(float maxHP, float movespeed, float attackspeed, float attackpoint, float maxStamina)
        {
            this.hp = maxHP;
            this.maxHP = maxHP; // armor
            this.armor = 0;
            this.movespeed = movespeed;
            this.attackSpeed = attackspeed;
            this.attackPoint = attackpoint;
            this.stamina = maxStamina;
            this.maxStamina = maxStamina; // armor
            this.critDamage = 1.5f;
        }
    }

    [System.Serializable]
    public struct EnemyStatus
    {
        public float hp;
        public float att;
        public float rangeAttack;
        public float moveSpeed;
        public TypeEnemy typeEnemy;
        public TypeMovementEnemy typeMovementEnemy;
    }

    // =============================================================
    // Game Save
    // =============================================================
    [System.Serializable]
    public class GameState
    {
        public PlayerStatus player;
        public Weapon? weapon;
        public List<Item> craftedItem;
        public  InventorySlot[] InventoryPlayer; 
        public string currentScene;
        public string lastSaveScene;
        public int level;
        public float currentExp;
        // i want to add other
        // level, current weapon, gold, 
    }



    // =============================================================
    // EFFECTT
    // =============================================================

    public struct Knockback
    {

        public float knockbackStrength;
        public Vector2 direction;

        public Knockback(float knockbackStrength, Vector2 dir)
        {
            this.knockbackStrength = knockbackStrength;
            this.direction = dir;
        }
    }

    // =============================================================
    // SYSTEM
    // =============================================================
    [System.Serializable]
    public class ObjectPolling
    {
        public int count;
        public GameObject gameObject;
    }
}


