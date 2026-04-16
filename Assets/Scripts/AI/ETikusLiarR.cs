using UnityEngine;
using data.structs;
using System.Collections;
using Pathfinding;
using Player.script;
using System.Collections.Generic;
// using System.Numerics;
public class ETikusLiarR : MonoBehaviour
{
    [SerializeField] EnemyStatus statEnemy, baseInit;
    [SerializeField] GameObject prefabBullet;
    public bool canMove, canAttack = true, isAlive = true, isVert = true;
    float delayHit = 1f, moveAtDir = 3f, projectileSpeed = 10f;
    public Transform target;

    bool isAttacking = false;
    int level;
    Coroutine chasingRoutine;

    // ==========================
    // COMPONENT HELPER
    // ==========================
    AIPath aIPath;
    EnemyPathFinderCust enemyPathFinderCust;
    EnemyDropSystem itemDropScript;
    void Awake()
    {
        aIPath = gameObject.GetComponent<AIPath>();
        itemDropScript = gameObject.GetComponent<EnemyDropSystem>();
        enemyPathFinderCust = gameObject.GetComponent<EnemyPathFinderCust>();
        Init();
    }
    void Start()
    {
        target = PlayerHit.Instance.transform;
        canAttack = true;

        if (chasingRoutine != null)
            StopCoroutine(chasingRoutine);
        chasingRoutine = StartCoroutine(StopAndChasing());
    }

    void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);
        statEnemy.hp = baseInit.hp + (level * 30);
        statEnemy.att = baseInit.att + (level * 10);
        statEnemy.moveSpeed = baseInit.moveSpeed + (level * 0.01f);

        Debug.Log($"{gameObject.name} Sudah dihitung berdasarkan level");
    }

    void OnEnable()
    {
        GameEvents.OnEnemyGetDamage += GetDamage;
        GameEvents.OnPlayerDead += TurnOffEnemy;
        GameEvents.CalculateEnemyStatByMapLevel += CalculatedStatEnemy;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyGetDamage -= GetDamage;
        GameEvents.OnPlayerDead -= TurnOffEnemy;
        GameEvents.CalculateEnemyStatByMapLevel -= CalculatedStatEnemy;
    }

    void GetDamage(string instanceID, float d)
    {
        if (!isAlive) return;
        if (gameObject.GetInstanceID().ToString() != instanceID) return;
        if (statEnemy.hp <= 0) return;

        statEnemy.hp -= d;

        if (statEnemy.hp <= 0)
            Dead();
    }


    void TurnOffEnemy()
    {
        // isCanChasing = false;
        target = null;
        canAttack = false;
        SetMovePathF(false);
        aIPath.maxSpeed = 0;
        aIPath.enabled = false;
    }

    void Dead()
    {
        if (!isAlive) return;
        isAlive = false;
        SetMovePathF(false);

        if (itemDropScript != null) StartCoroutine(itemDropScript.StartDrop()); // drop item

        MapIdentity.Instance.DecreaseEnemyCount();
        MapIdentity.Instance.SpawnObjectExp(transform, (baseInit.hp + (level * 14)) / 4);

        gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        Destroy(gameObject);
    }


    void Init()
    {
        // statEnemy.hp = 80;
        statEnemy.hp = 120;
        statEnemy.att = 30;
        statEnemy.rangeAttack = 0.8f;
        statEnemy.moveSpeed = 3.8f;
        statEnemy.typeEnemy = TypeEnemy.Mele;
        statEnemy.typeMovementEnemy = TypeMovementEnemy.Chasing;

        // setup to main settting
        aIPath.maxSpeed = statEnemy.moveSpeed;
        baseInit = statEnemy;
    }

    // slow, and will stop after 3s chasing dan 0.8 not move
    IEnumerator StopAndChasing()
    {
        while (isAlive)
        {
            // Do chase
            float time = 0;
            enemyPathFinderCust.aIDestinationSetter.targetPos = GetDirection();
            canMove = true;
            aIPath.canMove = canMove;

            while (!aIPath.reachedDestination && time < 1.8f)
            {
                time += Time.deltaTime;
                Debug.Log("Belummm--");
                yield return null;
            }

            // yield return new WaitForSeconds(3f);

            Debug.Log("SAMPAI--");
            // Do stop
            canMove = false;
            aIPath.canMove = canMove;

            // attack
            Attack();

            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        Debug.Log("SPAWNNN PREFAB ENEMY");
        SpawnPrefabProjectile();

        yield return new WaitForSeconds(0.5f); // cooldown attack

        canAttack = true;
        isAttacking = false;
    }


    void Attack()
    {
        if (!canAttack || isAttacking) return;
        canAttack = false;

        StartCoroutine(AttackRoutine());
        // StartCoroutine(DelayBeforeHitAgain());

        // SpawnPrefab
        // Debug.Log("SPAWNNN PREFAB ENEMY");
        // SpawnPrefabProjectile();
    }

    void SpawnPrefabProjectile()
    {
        Debug.Log("SPAWNN");
        List<float> angles = new List<float> { 0, 180 };

        if (isVert)
        {
            for (int i = 0; i < angles.Count; i++)
            {
                GameObject projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
                ProjectileEnemy projScript1 = projectile.GetComponent<ProjectileEnemy>();
                Vector3 direction = Vector3.up;
                projScript1.Init(direction, statEnemy.att, projectileSpeed, angles[i]);
            }
            //  projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
            //  projScript1 = projectile.GetComponent<ProjectileEnemy>();
            //  direction = Vector3.up;
            // projScript1.Init(direction, statEnemy.att, 10f, angles[1]);
            // }
        }
        else
        {
            for (int i = 0; i < angles.Count; i++)
            {
                GameObject projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
                ProjectileEnemy projScript1 = projectile.GetComponent<ProjectileEnemy>();
                Vector3 direction = Vector3.right;
                projScript1.Init(direction, statEnemy.att, 10f, angles[i]);
            }
            //  projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
            //  projScript1 = projectile.GetComponent<ProjectileEnemy>();
            //  direction = Vector3.right;
            // projScript1.Init(direction, statEnemy.att, 10f, angles[1]);
            // }
        }

        isVert = !isVert;
        canAttack = true;
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, statEnemy.rangeAttack);
    }

    public void SetMovePathF(bool state)
    {
        aIPath.canMove = state;
    }

    Vector3 GetDirection(float move = 2f)
    {
        // check nearest (hor/vert)
        float dx = Mathf.Abs(target.position.x - transform.position.x);
        float dy = Mathf.Abs(target.position.y - transform.position.y);
        Vector3 nextLoc = transform.position;

        if (dx > dy)
        {
            Debug.Log("Lebih dekat lewat horizontal");
            // isVert = false;
            if (target.position.x > transform.position.x) nextLoc.x += move;
            else nextLoc.x -= move;
        }
        else
        {
            // isVert = true;
            Debug.Log("Lebih dekat lewat vertical");
            if (target.position.y > transform.position.y) nextLoc.y += move;
            else nextLoc.y -= move;
        }

        return nextLoc;
        // target.position // 13, 0, 0 (player)
        // transform.position // 8, 3, 0
    }



}
