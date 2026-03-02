using UnityEngine;
using data.structs;
using System.Collections.Generic;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] List<GameObject> projectiles;
    [SerializeField] LayerMask enemyLayer;
    float attackTimer;
    bool canShoot = false;
    Weapon weapon;
    [SerializeField] GameObject projectile;
    PlayerStat playerStat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapon = new Weapon("Bow", 5, 3, 10, 4, Weapons.Bow);
        playerStat = gameObject.GetComponent<PlayerStat>();



    }
    void SetIsCanShot(bool stateMove)
    {
        if (stateMove)
        {
            canShoot = false;
        }
        else canShoot = true;
    }


    // suscribe event 
    void OnEnable()
    {
        GameEvents.OnPlayerMove += SetIsCanShot;
    }
    void OnDisable()
    {

    }




    void Update()
    {
        attackTimer += Time.deltaTime;

        Transform target = FindClosestEnemy();

        if (target == null)
        {
            Debug.Log("Tidak ada enemy dalam range");
        }
       
        if (target != null && attackTimer >= playerStat.playerStatus.attackSpeed)
        {
            Shoot(target);
            attackTimer = 0f;
        }
    }

    Transform FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weapon.range, enemyLayer);

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        if(!canShoot) return;
        
        GameObject arrow = Instantiate(projectiles[((int)weapon.attachmentWeapon)], transform.position, Quaternion.identity);
        Vector3 direction = (target.position - transform.position).normalized;
        ArrowSystem projectile = arrow.GetComponent<ArrowSystem>();
        projectile.Init(direction, weapon.damage, weapon.lifetime, weapon.damage);
    }

}
