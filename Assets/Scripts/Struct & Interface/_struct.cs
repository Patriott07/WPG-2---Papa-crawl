using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace data.structs
{
    public enum TypeOfItem { just_item, use_item, equipment };

    public enum Weapons
    {
        Slingshot, Bow, Dagger, Crossbow
    }
    public enum AttachmentWeaponType
    {
        Damage,
        AttackSpeed,
        CriticalDamage,
        CriticalRate,
        Lifesteal,
        Penetration,
    }

    [System.Serializable]
    public struct Weapon
    {
        public string name;
        public float speed;
        public float damage;
        public float range;
        public float lifetime;
        public int penetration;
        public int level;
        public Weapons attachmentWeapon;

        public List<AttachmentWeaponType> attachments;
        public Weapon(string name, int speed, float damage, float range, float lifetime, Weapons attachmentWeapon)
        {
            this.name = name;
            this.speed = speed;
            this.damage = damage;
            this.range = range;
            this.lifetime = lifetime;
            this.level = 1;
            this.penetration = 0;
            this.attachments = new List<AttachmentWeaponType>();
            this.attachmentWeapon = attachmentWeapon;
        }
    }

    [System.Serializable]
    public struct CollectItem
    {
        public string name;
        public int qty;
        public int maxStack;
        public RawImage artUI;
        public Sprite artSprite;

        public CollectItem(string name, int maxStack, Sprite artSprite, RawImage artUI)
        {
            this.name = name;
            this.qty = 1;
            this.maxStack = maxStack;
            this.artUI = artUI;
            this.artSprite = artSprite;
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

        public PlayerStatus(float maxHP, float movespeed, float attackspeed, float attackpoint, float maxStamina)
        {
            this.hp = maxHP;
            this.maxHP = maxHP;
            this.armor = 0;
            this.movespeed = movespeed;
            this.attackSpeed = attackspeed;
            this.attackPoint = attackpoint;
            this.stamina = maxStamina;
            this.maxStamina = maxStamina;
        }
    }
}


