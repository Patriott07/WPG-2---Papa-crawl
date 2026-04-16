using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Player.script;
using data.structs;

public class ETikusBanditRange : MonoBehaviour
{
    [Header("=== STATS ===")]
    [SerializeField] EnemyStatus statEnemy, baseInit;

    [Header("=== REFERENCES ===")]
    [SerializeField] GameObject prefabBullet;

    public Transform target;

    // STATE
    bool isAlive = true;
    bool isAttacking = false;

    float projectileSpeed = 15f;
    float radiusPathfinderPos = 1.2f;

    int level;

    // COMPONENT
    AIPath aiPath;
    EnemyPathFinderCust enemyPathFinderCust;

    Coroutine mainRoutine;
      EnemyDropSystem itemDropScript;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        enemyPathFinderCust = GetComponent<EnemyPathFinderCust>();
        itemDropScript = gameObject.GetComponent<EnemyDropSystem>();
        Init();
    }

    void Start()
    {
        target = PlayerHit.Instance.transform;

        if (mainRoutine != null)
            StopCoroutine(mainRoutine);

        mainRoutine = StartCoroutine(MainAI());
    }

    void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);
        statEnemy.hp = baseInit.hp + (level * 20);
        statEnemy.att = baseInit.att + (level * 8);
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

    // ==========================
    // MAIN AI LOOP
    // ==========================
    IEnumerator MainAI()
    {
        while (isAlive)
        {
            yield return StartCoroutine(DoChase());
            yield return StartCoroutine(DoAttack());

            yield return new WaitForSeconds(0.5f);
        }
    }

    // ==========================
    // CHASE
    // ==========================
    IEnumerator DoChase()
    {
        float time = 0f;

        aiPath.canMove = true;
        enemyPathFinderCust.aIDestinationSetter.targetPos = GetDirection(radiusPathfinderPos);

        while (!aiPath.reachedDestination && time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        aiPath.canMove = false;
    }

    // ==========================
    // ATTACK
    // ==========================
    IEnumerator DoAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        StartCoroutine(SpawnProjectile());

        yield return new WaitForSeconds(2.1f); // cooldown
        isAttacking = false;
    }

    // ==========================
    // SHOOT
    // ==========================
    IEnumerator SpawnProjectile()
    {
        List<float> angles = new List<float> { 0, 0 };

        foreach (float angle in angles)
        {
            GameObject projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
            ProjectileEnemy proj = projectile.GetComponent<ProjectileEnemy>();

            Vector3 dir = (target.position - transform.position).normalized;
            proj.Init(dir, statEnemy.att, projectileSpeed, angle);

            yield return new WaitForSeconds(0.2f);
        }
    }

    // ==========================
    // DAMAGE SYSTEM
    // ==========================
    void GetDamage(string instanceID, float d)
    {
        if (!isAlive) return;
        if (gameObject.GetInstanceID().ToString() != instanceID) return;

        statEnemy.hp -= d;

        if (statEnemy.hp <= 0)
            Dead();
    }

    void Dead()
    {
        if (!isAlive) return;
        isAlive = false;
        aiPath.canMove = false;
        GetComponent<SpriteRenderer>().color = Color.blue;

        MapIdentity.Instance.DecreaseEnemyCount();
          if(itemDropScript != null) StartCoroutine(itemDropScript.StartDrop()); // drop item
        MapIdentity.Instance.SpawnObjectExp(transform, (baseInit.hp + (level * 14)) / 4);

        StopAllCoroutines();
        Destroy(gameObject);
    }

    void TurnOffEnemy()
    {
        // isCanChasing = false;
        target = null;
        isAlive = false;
        SetMovePathF(false);
        aiPath.maxSpeed = 0;
        aiPath.enabled = false;
    }

    // ==========================
    // INIT
    // ==========================
    void Init()
    {
        statEnemy.hp = 135;
        statEnemy.att = 45;
        statEnemy.moveSpeed = 2.2f;

        baseInit = statEnemy;

        aiPath.maxSpeed = statEnemy.moveSpeed;
    }

    // ==========================
    // RANDOM MOVE TARGET
    // ==========================
    Vector3 GetDirection(float radius)
    {
        return new Vector3(
            transform.position.x + Random.Range(-radius, radius),
            transform.position.y + Random.Range(-radius, radius),
            0
        );
    }

    // ==========================
    // DEBUG
    // ==========================
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    public void SetMovePathF(bool state)
    {
        aiPath.canMove = state;
    }
}